using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;
using Project.DAL.Models;

namespace Project.Controllers;
[ApiController]
public class AdminModuleController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminModuleController(AppDbContext context)
    {
        _context = context;
    }

    // GET /admin/dashboard
    [HttpGet("/admin/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var usersCount = await _context.Users.CountAsync();
        var doctorsCount = await _context.Doctors.CountAsync();
        var patientsCount = await _context.Patients.CountAsync();
        var appointmentsCount = await _context.Appointments.CountAsync();

        return Ok(new
        {
            Users = usersCount,
            Doctors = doctorsCount,
            Patients = patientsCount,
            Appointments = appointmentsCount
        });
    }

    // GET /admin/users
    [HttpGet("/admin/users")]
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    public record UpdateUserRequest(string? FullName, string? Phone, string? Role, bool? IsActive);

    // PUT /admin/users/{id}
    [HttpPut("/admin/users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.Phone))
            user.Phone = request.Phone;

        if (!string.IsNullOrWhiteSpace(request.Role))
            user.Role = request.Role;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();

        return Ok(user);
    }

    // DELETE /admin/users/{id}
    [HttpDelete("/admin/users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET /admin/doctors
    [HttpGet("/admin/doctors")]
    public async Task<IActionResult> GetDoctors()
    {
        var doctors = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .ToListAsync();

        return Ok(doctors.Select(d => new
        {
            d.Id,
            d.User.FullName,
            d.User.Email,
            d.User.IsActive,
            Specialization = d.Specialization != null ? d.Specialization.Name : null
        }));
    }

    // POST /admin/doctors/approve/{id}
    [HttpPost("/admin/doctors/approve/{id:int}")]
    public async Task<IActionResult> ApproveDoctor(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.User.IsActive = true;
        await _context.SaveChangesAsync();

        // notify doctor about approval
        _context.Notifications.Add(new Notification
        {
            UserId = doctor.User.Id,
            Channel = "System",
            ScheduledAt = DateTime.UtcNow,
            Status = "Pending"
        });
        await _context.SaveChangesAsync();

        return Ok(new
        {
            doctor.Id,
            doctor.User.FullName,
            doctor.User.Email,
            doctor.User.IsActive
        });
    }

    // GET /admin/appointments
    [HttpGet("/admin/appointments")]
    public async Task<IActionResult> GetAllAppointments(
        [FromQuery] string? status,
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId)
    {
        var query = _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(a => a.Status == parsedStatus);
            }
        }


        if (doctorId.HasValue)
        {
            query = query.Where(a => a.DoctorId == doctorId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        var list = await query
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return Ok(list.Select(a => new
        {
            a.Id,
            a.StartAt,
            a.EndAt,
            a.Status,
            DoctorName = a.Doctor.User.FullName,
            PatientName = a.Patient.User.FullName
        }));
    }
}


