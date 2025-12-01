using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers;

[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /appointment/check
    [HttpGet("/appointment/check")]
    public async Task<IActionResult> CheckAvailability(
        [FromQuery] int doctorId,
        [FromQuery] DateTime startAt,
        [FromQuery] DateTime endAt)
    {
        if (endAt <= startAt)
        {
            return BadRequest(new { message = "End time must be after start time" });
        }

        var conflict = await _context.Appointments.AnyAsync(a =>
            a.DoctorId == doctorId &&
            a.Status != "Canceled" &&
            a.Status != "Completed" &&
            ((startAt >= a.StartAt && startAt < a.EndAt) ||
             (endAt > a.StartAt && endAt <= a.EndAt) ||
             (startAt <= a.StartAt && endAt >= a.EndAt)));

        return Ok(new { available = !conflict });
    }

    public record AddAppointmentRequest(
        int DoctorId,
        int PatientId,
        DateTime StartAt,
        DateTime EndAt,
        string? Notes);

    // POST /appointment/add
    [HttpPost("/appointment/add")]
    public async Task<IActionResult> AddAppointment([FromBody] AddAppointmentRequest request)
    {
        if (request.EndAt <= request.StartAt)
        {
            return BadRequest(new { message = "End time must be after start time" });
        }

        var availableResult = await CheckAvailability(request.DoctorId, request.StartAt, request.EndAt) as OkObjectResult;
        if (availableResult?.Value is not null)
        {
            var availableProp = availableResult.Value.GetType().GetProperty("available");
            var available = availableProp != null && (bool)(availableProp.GetValue(availableResult.Value) ?? false);
            if (!available)
            {
                return Conflict(new { message = "Time slot is not available" });
            }
        }

        var appointment = new Appointment
        {
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Status = "Pending",
            Notes = request.Notes ?? string.Empty
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // create notifications for doctor and patient
        var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == request.DoctorId);
        var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == request.PatientId);

        if (doctor?.User != null)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = doctor.User.Id,
                AppointmentId = appointment.Id,
                Channel = "System",
                ScheduledAt = DateTime.UtcNow,
                Status = "Pending"
            });
        }

        if (patient?.User != null)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = patient.User.Id,
                AppointmentId = appointment.Id,
                Channel = "System",
                ScheduledAt = DateTime.UtcNow,
                Status = "Pending"
            });
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyAppointments), new { patientId = request.PatientId }, appointment);
    }

    // POST /appointment/cancel/{id}
    [HttpPost("/appointment/cancel/{id:int}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        if (appointment.StartAt <= DateTime.UtcNow.AddHours(24))
        {
            return BadRequest(new { message = "Cannot cancel within 24 hours of the appointment" });
        }

        appointment.Status = "Canceled";
        await _context.SaveChangesAsync();

        // simple notification: appointment canceled
        var participants = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (participants != null)
        {
            if (participants.Doctor?.User != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = participants.Doctor.User.Id,
                    AppointmentId = participants.Id,
                    Channel = "System",
                    ScheduledAt = DateTime.UtcNow,
                    Status = "Pending"
                });
            }
            if (participants.Patient?.User != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = participants.Patient.User.Id,
                    AppointmentId = participants.Id,
                    Channel = "System",
                    ScheduledAt = DateTime.UtcNow,
                    Status = "Pending"
                });
            }
            await _context.SaveChangesAsync();
        }

        return Ok(appointment);
    }

    // POST /appointment/confirm/{id}
    [HttpPost("/appointment/confirm/{id:int}")]
    public async Task<IActionResult> ConfirmAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        appointment.Status = "Confirmed";
        await _context.SaveChangesAsync();

        var participants = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (participants != null)
        {
            if (participants.Patient?.User != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = participants.Patient.User.Id,
                    AppointmentId = participants.Id,
                    Channel = "System",
                    ScheduledAt = DateTime.UtcNow,
                    Status = "Pending"
                });
            }
            await _context.SaveChangesAsync();
        }

        return Ok(appointment);
    }

    public record RescheduleAppointmentRequest(DateTime StartAt, DateTime EndAt, string? Notes);

    // POST /appointment/reschedule/{id}
    [HttpPost("/appointment/reschedule/{id:int}")]
    public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequest request)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        if (request.EndAt <= request.StartAt)
        {
            return BadRequest(new { message = "End time must be after start time" });
        }

        var conflict = await _context.Appointments.AnyAsync(a =>
            a.Id != id &&
            a.DoctorId == appointment.DoctorId &&
            a.Status != "Canceled" &&
            a.Status != "Completed" &&
            ((request.StartAt >= a.StartAt && request.StartAt < a.EndAt) ||
             (request.EndAt > a.StartAt && request.EndAt <= a.EndAt) ||
             (request.StartAt <= a.StartAt && request.EndAt >= a.EndAt)));

        if (conflict)
        {
            return Conflict(new { message = "New time slot is not available" });
        }

        appointment.StartAt = request.StartAt;
        appointment.EndAt = request.EndAt;
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            appointment.Notes = request.Notes;
        }

        await _context.SaveChangesAsync();

        var participants = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (participants != null)
        {
            if (participants.Patient?.User != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = participants.Patient.User.Id,
                    AppointmentId = participants.Id,
                    Channel = "System",
                    ScheduledAt = DateTime.UtcNow,
                    Status = "Pending"
                });
            }
            await _context.SaveChangesAsync();
        }

        return Ok(appointment);
    }

    // GET /appointment/my
    [HttpGet("/appointment/my")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] int patientId)
    {
        var now = DateTime.UtcNow;
        var appointments = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.StartAt)
            .Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.Notes,
                DoctorName = a.Doctor.User.FullName,
                IsUpcoming = a.StartAt >= now
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // GET /appointment/today
    [HttpGet("/appointment/today")]
    public async Task<IActionResult> GetTodayAppointments([FromQuery] int doctorId)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var appointments = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId && a.StartAt >= today && a.StartAt < tomorrow)
            .OrderBy(a => a.StartAt)
            .Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.Notes,
                PatientName = a.Patient.User.FullName
            })
            .ToListAsync();

        return Ok(appointments);
    }
}


