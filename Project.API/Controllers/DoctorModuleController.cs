using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Controllers;

[ApiController]
public class DoctorModuleController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorModuleController(AppDbContext context)
    {
        _context = context;
    }

    // GET /doctor/dashboard
    [HttpGet("/doctor/dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] int doctorId)
    {
        var today = DateTime.UtcNow.Date;
        var weekEnd = today.AddDays(7);

        var todayAppointments = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId && a.StartAt.Date == today)
            .ToListAsync();

        var weekAppointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.StartAt >= today && a.StartAt < weekEnd)
            .CountAsync();

        return Ok(new
        {
            TodayCount = todayAppointments.Count,
            ThisWeekCount = weekAppointments,
            TodayAppointments = todayAppointments.Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                PatientName = a.Patient.User.FullName
            })
        });
    }

    // GET /doctor/appointments
    [HttpGet("/doctor/appointments")]
    public async Task<IActionResult> GetAppointments([FromQuery] int doctorId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return Ok(appointments.Select(a => new
        {
            a.Id,
            a.StartAt,
            a.EndAt,
            a.Status,
            a.Notes,
            PatientName = a.Patient.User.FullName
        }));
    }

    // GET /doctor/availability
    [HttpGet("/doctor/availability")]
    public async Task<IActionResult> GetAvailability([FromQuery] int doctorId)
    {
        var availability = await _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        return Ok(availability);
    }

    public record AvailabilityRequest(
        string DayOfWeek,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int SlotMinutes);

    // POST /doctor/availability
    [HttpPost("/doctor/availability")]
    public async Task<IActionResult> AddOrUpdateAvailability(
        [FromQuery] int doctorId,
        [FromBody] AvailabilityRequest request)
    {
        if (request.EndTime <= request.StartTime)
        {
            return BadRequest(new { message = "End time must be after start time" });
        }

        var existing = await _context.DoctorAvailabilities.FirstOrDefaultAsync(a =>
            a.DoctorId == doctorId &&
            a.DayOfWeek == request.DayOfWeek &&
            a.StartTime == request.StartTime &&
            a.EndTime == request.EndTime);

        if (existing == null)
        {
            existing = new Models.DoctorAvailability
            {
                DoctorId = doctorId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                SlotMinutes = request.SlotMinutes
            };
            _context.DoctorAvailabilities.Add(existing);
        }
        else
        {
            existing.SlotMinutes = request.SlotMinutes;
        }

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // POST /doctor/availability/delete/{id}
    [HttpPost("/doctor/availability/delete/{id:int}")]
    public async Task<IActionResult> DeleteAvailability(int id)
    {
        var availability = await _context.DoctorAvailabilities.FindAsync(id);
        if (availability == null)
        {
            return NotFound();
        }

        _context.DoctorAvailabilities.Remove(availability);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET /doctor/profile
    [HttpGet("/doctor/profile")]
    public async Task<IActionResult> GetProfile([FromQuery] int doctorId)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            doctor.Id,
            doctor.User.FullName,
            doctor.User.Email,
            doctor.User.Phone,
            doctor.ClinicAddress,
            doctor.Bio,
            Specialization = doctor.Specialization != null ? doctor.Specialization.Name : null
        });
    }

    public record EditDoctorProfileRequest(
        string? FullName,
        string? Phone,
        string? ClinicAddress,
        string? Bio,
        int? SpecializationId);

    // POST /doctor/profile
    [HttpPost("/doctor/profile")]
    public async Task<IActionResult> EditProfile([FromQuery] int doctorId, [FromBody] EditDoctorProfileRequest request)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
            doctor.User.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.Phone))
            doctor.User.Phone = request.Phone;

        if (!string.IsNullOrWhiteSpace(request.ClinicAddress))
            doctor.ClinicAddress = request.ClinicAddress;

        if (!string.IsNullOrWhiteSpace(request.Bio))
            doctor.Bio = request.Bio;

        if (request.SpecializationId.HasValue)
            doctor.SpecializationId = request.SpecializationId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            doctor.Id,
            doctor.User.FullName,
            doctor.User.Email,
            doctor.User.Phone,
            doctor.ClinicAddress,
            doctor.Bio,
            doctor.SpecializationId
        });
    }
}


