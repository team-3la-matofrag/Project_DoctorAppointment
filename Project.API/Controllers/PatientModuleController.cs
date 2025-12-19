using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;

namespace Project.Controllers;

[ApiController]
public class PatientModuleController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientModuleController(AppDbContext context)
    {
        _context = context;
    }

    // GET /patient/dashboard
    [HttpGet("/patient/dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] int patientId)
    {
        var now = DateTime.UtcNow;
        var appointments = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.StartAt)
            .ToListAsync();

        var upcoming = appointments.Where(a => a.StartAt >= now).ToList();
        var history = appointments.Where(a => a.StartAt < now).ToList();

        return Ok(new
        {
            UpcomingCount = upcoming.Count,
            HistoryCount = history.Count,
            Upcoming = upcoming.Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                DoctorName = a.Doctor.User.FullName
            }),
            History = history.Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                DoctorName = a.Doctor.User.FullName
            })
        });
    }

    // GET /patient/appointments
    [HttpGet("/patient/appointments")]
    public async Task<IActionResult> GetAppointments([FromQuery] int patientId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return Ok(appointments.Select(a => new
        {
            a.Id,
            a.StartAt,
            a.EndAt,
            a.Status,
            a.Notes,
            DoctorName = a.Doctor.User.FullName
        }));
    }

    // GET /patient/book/{doctorId}
    [HttpGet("/patient/book/{doctorId:int}")]
    public async Task<IActionResult> BookingPage(int doctorId)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Availabilities)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        var slots = new List<object>();
        foreach (var availability in doctor.Availabilities)
        {
            var start = availability.StartTime;
            while (start + TimeSpan.FromMinutes(availability.SlotMinutes) <= availability.EndTime)
            {
                var end = start + TimeSpan.FromMinutes(availability.SlotMinutes);
                slots.Add(new
                {
                    availability.DayOfWeek,
                    Start = start,
                    End = end
                });
                start = end;
            }
        }

        return Ok(new
        {
            DoctorId = doctor.Id,
            DoctorName = doctor.User.FullName,
            Slots = slots
        });
    }
}


