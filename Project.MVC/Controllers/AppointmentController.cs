using Microsoft.AspNetCore.Mvc;
using Project.BLL.DTOs;
using Project.DAL.Models;
using Project.MVC.Services;

namespace Project.MVC.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly ApiService _api;

        public AppointmentController(ApiService api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpGet]
        public IActionResult Book()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(int doctor, DateTime date, string message)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");
            if (doctor == 0)
            {
                ModelState.AddModelError("", "Please select a doctor.");
                return View();
            }

            try
            {
                var userId = GetCurrentUserId();
                // Find patient ID associated with user
                var patient = await _api.GetAsync<PatientDto>($"/api/patients/profile/{userId}"); 
                if (patient == null) throw new Exception("Patient profile not found");

                var dto = new CreateAppointmentDto
                {
                    DoctorId = doctor,
                    PatientId = patient.Id,
                    StartAt = date,
                    EndAt = date.AddMinutes(30), // Default duration
                    Notes = message
                };

                await _api.PostAsync("/api/appointments", dto);
                
                TempData["Success"] = "Appointment requested successfully!";
                return RedirectToAction("MyAppointments");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpecialty(string specialty)
        {
            var doctors = await _api.GetAsync<List<DoctorDto>>("/api/doctors");
            var filtered = doctors.Where(d => string.Equals(d.Specialization, specialty, StringComparison.OrdinalIgnoreCase)).Select(d => new { d.Id, d.FullName });
            return Json(filtered);
        }
        public IActionResult MyAppointments()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");


            int userId = GetCurrentUserId();
            return View();

        }
      
    }
}
