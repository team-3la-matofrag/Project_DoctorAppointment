using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // GET /login
    [HttpGet("/login")]
    public IActionResult LoginPage()
    {
        // In a pure API this would normally return metadata or be omitted.
        return Ok(new { message = "Login page placeholder" });
    }

    public record LoginRequest(string Email, string Password);

    // POST /login
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid credentials or inactive account" });
        }

        // NOTE: For simplicity we just return basic user info.
        // In a real app you would generate a JWT or cookie.
        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.Role
        });
    }

    public record RegisterPatientRequest(
        string FullName,
        string Email,
        string Password,
        string Phone,
        DateTime Dob,
        string Gender,
        string? Notes);

    // GET /register
    [HttpGet("/register")]
    public IActionResult RegisterPatientPage()
    {
        return Ok(new { message = "Register patient page placeholder" });
    }

    // POST /register
    [HttpPost("/register")]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.Password, // TODO: hash password in a real app
            Role = "Patient",
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var patient = new Patient
        {
            User = user,
            DOB = request.Dob,
            Gender = request.Gender,
            Notes = request.Notes ?? string.Empty
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(RegisterPatient), new { id = patient.Id }, new
        {
            patient.Id,
            user.FullName,
            user.Email,
            user.Role
        });
    }

    public record RegisterDoctorRequest(
        string FullName,
        string Email,
        string Password,
        string Phone,
        int? SpecializationId,
        string? Bio,
        string? ClinicAddress);

    // GET /register-doctor
    [HttpGet("/register-doctor")]
    public IActionResult RegisterDoctorPage()
    {
        return Ok(new { message = "Register doctor page placeholder" });
    }

    // POST /register-doctor
    [HttpPost("/register-doctor")]
    public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = "Doctor",
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
            IsActive = false // requires admin approval
        };

        var doctor = new Doctor
        {
            User = user,
            SpecializationId = request.SpecializationId,
            Bio = request.Bio ?? string.Empty,
            ClinicAddress = request.ClinicAddress ?? string.Empty
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(RegisterDoctor), new { id = doctor.Id }, new
        {
            doctor.Id,
            user.FullName,
            user.Email,
            user.Role,
            RequiresApproval = true
        });
    }

    // POST /logout
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        // In a stateless API this is usually handled on the client by discarding the token.
        return Ok(new { message = "Logged out" });
    }
}


