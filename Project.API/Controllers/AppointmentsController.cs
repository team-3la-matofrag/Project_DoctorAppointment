using Microsoft.AspNetCore.Mvc;
using Project.DAL.Models;
using Project.BLL.DTOs;
using Project.BLL.Interfaces;

namespace Project.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelAsync(id);
            return Ok();
        }
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _service.ConfirmAsync(id);
            return Ok(new { message = "Appointment confirmed successfully" });
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                await _service.CompleteAsync(id);
                return Ok(new { message = "appointment Completed Successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message= ex.Message });
            }
        }


        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> Doctor(int doctorId)
            => Ok(await _service.GetDoctorAppointmentsAsync(doctorId));

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> Patient(int patientId)
            => Ok(await _service.GetPatientAppointmentsAsync(patientId));
    }


}
