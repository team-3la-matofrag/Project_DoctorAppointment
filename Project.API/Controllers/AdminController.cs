using Microsoft.AspNetCore.Mvc;
using Project.BLL.Interfaces;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
        => Ok(await _adminService.GetDashboardAsync());

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctors()
        => Ok(await _adminService.GetAllDoctorsAsync());

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
        => Ok(await _adminService.GetAllPatientsAsync());

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments()
        => Ok(await _adminService.GetAllAppointmentsAsync());

    [HttpPost("appointments/{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        await _adminService.ForceCancelAppointmentAsync(id);
        return Ok();
    }
}