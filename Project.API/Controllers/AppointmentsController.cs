using Microsoft.AspNetCore.Mvc;
using Project.DAL.Models;
using Project.BLL.DTOs;
using Project.BLL.Interfaces;

namespace Project.API.Controllers
{
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }


        [HttpGet("/appointment/check")]
        public async Task<IActionResult> CheckAvailability(
            int doctorId,
            DateTime startAt,
            DateTime endAt)
        {
            var available = await _service.CheckAvailabilityAsync(doctorId, startAt, endAt);
            return Ok(new { available });
        }

        [HttpPost("/appointment/add")]
        public async Task<IActionResult> Add([FromBody] AppointmentDto dto)
        {
            try
            {
                var result = await _service.AddAppointmentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("/appointment/cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelAsync(id);
            return Ok();
        }

        [HttpPost("/appointment/confirm/{id:int}")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _service.ConfirmAsync(id);
            return Ok();
        }

        [HttpPost("/appointment/reschedule/{id:int}")]
        public async Task<IActionResult> Reschedule(
            int id,
            DateTime startAt,
            DateTime endAt,
            string? notes)
        {
            await _service.RescheduleAsync(id, startAt, endAt, notes);
            return Ok();
        }

        [HttpGet("/appointment/my")]
        public async Task<IActionResult> My(int patientId)
        {
            return Ok(await _service.GetMyAppointmentsAsync(patientId));
        }

        [HttpGet("/appointment/today")]
        public async Task<IActionResult> Today(int doctorId)
        {
            return Ok(await _service.GetTodayAppointmentsAsync(doctorId));
        }
    }
}
