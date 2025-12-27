using Microsoft.AspNetCore.Mvc;
using Project.MVC.Services;

namespace Project.MVC.Controllers
{
    public class PatientController : BaseController
    {
        private readonly ApiService _api;

        public PatientController(ApiService api)
        {
            _api = api;
        }

        public IActionResult Dashboard()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");
            // In a real app, you'd fetch patient summary data here
            return View();
        }

        public async Task<IActionResult> Appointments()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            try
            {
                var userId = GetCurrentUserId();
                var patient = await _api.GetAsync<Project.BLL.DTOs.PatientDto>($"/api/patients/profile/{userId}");
                if (patient == null) return RedirectToAction("Login", "Account");

                var appointments = await _api.GetAsync<List<Models.PatientAppointmentViewModel>>($"/api/patients/{patient.Id}/appointments");
                return View(appointments ?? new List<Models.PatientAppointmentViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error fetching appointments: " + ex.Message;
                return View(new List<Models.PatientAppointmentViewModel>());
            }
        }

        public IActionResult Profile()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");
            return View();
        }
    }
}
