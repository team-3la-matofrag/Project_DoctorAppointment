using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers;

[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /
    [HttpGet("/")]
    public IActionResult Home()
    {
        return Ok(new { message = "Home page placeholder" });
    }

    // GET /doctors
    [HttpGet("/doctors")]
    public async Task<IActionResult> GetDoctors([FromQuery] int? specializationId)
    {
        var query = _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .AsQueryable();

        if (specializationId.HasValue)
        {
            query = query.Where(d => d.SpecializationId == specializationId.Value);
        }

        var doctors = await query
            .Select(d => new
            {
                d.Id,
                Name = d.User.FullName,
                d.User.Email,
                d.User.Phone,
                Specialization = d.Specialization != null ? d.Specialization.Name : null,
                d.ClinicAddress,
                d.Bio
            })
            .ToListAsync();

        return Ok(doctors);
    }

    // GET /doctors/search
    [HttpGet("/doctors/search")]
    public async Task<IActionResult> SearchDoctors(
        [FromQuery] string? name,
        [FromQuery] string? specialty,
        [FromQuery] string? city,
        [FromQuery] DateTime? availableDate)
    {
        var query = _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .Include(d => d.Availabilities)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(d => d.User.FullName.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(specialty))
        {
            query = query.Where(d => d.Specialization != null && d.Specialization.Name.Contains(specialty));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(d => d.ClinicAddress.Contains(city));
        }

        if (availableDate.HasValue)
        {
            var date = availableDate.Value.Date;
            query = query.Where(d =>
                d.Availabilities.Any(a => a.DayOfWeek == date.DayOfWeek.ToString()));
        }

        var doctors = await query
            .Select(d => new
            {
                d.Id,
                Name = d.User.FullName,
                d.User.Email,
                d.User.Phone,
                Specialization = d.Specialization != null ? d.Specialization.Name : null,
                d.ClinicAddress,
                d.Bio
            })
            .ToListAsync();

        return Ok(doctors);
    }

    // GET /doctors/details/{id}
    [HttpGet("/doctors/details/{id:int}")]
    public async Task<IActionResult> GetDoctorDetails(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .Include(d => d.Availabilities)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound();
        }

        // Build basic slots from availability (does not check actual booked appointments here)
        var slots = new List<object>();
        foreach (var availability in doctor.Availabilities)
        {
            var start = availability.StartTime;
            while (start + TimeSpan.FromMinutes(availability.SlotMinutes) <= availability.EndTime)
            {
                var end = start + TimeSpan.FromMinutes(availability.SlotMinutes);
                slots.Add(new
                {
                    DayOfWeek = availability.DayOfWeek,
                    Start = start,
                    End = end
                });
                start = end;
            }
        }

        return Ok(new
        {
            doctor.Id,
            Name = doctor.User.FullName,
            doctor.User.Email,
            doctor.User.Phone,
            Specialization = doctor.Specialization != null ? doctor.Specialization.Name : null,
            doctor.ClinicAddress,
            doctor.Bio,
            AvailableSlots = slots
        });
    }
}


