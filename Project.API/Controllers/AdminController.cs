using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.Interfaces;
using System.Threading.Tasks;

namespace Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return Ok(result);
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var result = await _adminService.GetAllDoctorsAsync();
            return Ok(result);
        }

        [HttpPost("doctors/{id}/activate")]
        public async Task<IActionResult> ActivateDoctor(int id)
        {
            await _adminService.ActivateDoctorAsync(id);
            return Ok(new { message = "Doctor activated successfully" });
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var result = await _adminService.GetAllPatientsAsync();
            return Ok(result);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var result = await _adminService.GetAllAppointmentsAsync();
            return Ok(result);
        }

        [HttpPost("appointments/{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            await _adminService.ForceCancelAppointmentAsync(id);
            return Ok(new { message = "Appointment cancelled successfully" });
        }
    }
}
