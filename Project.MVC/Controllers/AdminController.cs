using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.Services;



namespace Project.MVC.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApiService _apiService;

        public AdminController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var stats =
               await _apiService.GetAsync<AdminDashboard>("api/admin/dashboard");

            var recentAppointments =
                await _apiService.GetAsync<List<AdminAppointmentViewModel>>("api/admin/appointments");
            ViewBag.Stats = stats;
            ViewBag.RecentAppointments = recentAppointments;

            return View();
        }

        public IActionResult Users()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (!IsAdmin())
                return Unauthorized();

            var users =
                await _apiService.GetAsync<List<UserAdminViewModel>>("admin/users");

            return Json(users);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id,
            [FromBody] UserAdminViewModel model)
        {
            if (!IsAdmin())
                return Unauthorized();

            await _apiService.PutAsync($"admin/users/{id}", model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            await _apiService.DeleteAsync($"api/admin/users/{id}");
            return NoContent();
        }

        public IActionResult Doctors()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            if (!IsAdmin())
                return Unauthorized();

            var doctors =
                await _apiService.GetAsync<List<DoctorAdminViewModel>>("admin/doctors");

            return Json(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            await _apiService.PostAsync<object, object>(
                $"admin/doctors/approve/{id}", null!);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDoctorStatus(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            await _apiService.PostAsync<object, object>(
                $"api/doctors/{id}/toggle", null!);

            return Ok();
        }

        public IActionResult Appointments()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments(
            string? status,
            int? doctorId,
            int? patientId,
            DateTime? date)
        {
            if (!IsAdmin())
                return Unauthorized();

            var query =
                $"admin/appointments?status={status}&doctorId={doctorId}&patientId={patientId}&date={date}";

            var appointments =
                await _apiService.GetAsync<List<AdminAppointmentViewModel>>(query);

            return Json(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

           await _apiService.PostAsync<object, object>(
                $"admin/appointments/{id}/cancel", null!);

            return Ok();
        }
    }
}