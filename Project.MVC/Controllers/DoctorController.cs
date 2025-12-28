using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.Services;

namespace Project.MVC.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly ApiService _api;

        public DoctorController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (CurrentUser?.Role != "Doctor")
            {
                TempData["Error"] = "Access denied. This page is for doctors only.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = GetCurrentUserId();

                // Fetch doctor dashboard data from API
                var dashboardData = await _api.GetAsync<DoctorDashboardViewModel>(
                    $"/api/doctors/dashboard/{userId}");

                if (dashboardData == null)
                {
                    TempData["Warning"] = "Unable to load dashboard data.";
                    return View(new DoctorDashboardViewModel
                    {
                        Profile = new DoctorProfileViewModel
                        {
                            FullName = CurrentUser?.FullName ?? "Doctor"
                        },
                        Stats = new DoctorStatsViewModel(),
                        TodayAppointments = new List<DoctorAppointmentViewModel>()
                    });
                }

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                return View(new DoctorDashboardViewModel
                {
                    Profile = new DoctorProfileViewModel
                    {
                        FullName = CurrentUser?.FullName ?? "Doctor"
                    },
                    Stats = new DoctorStatsViewModel(),
                    TodayAppointments = new List<DoctorAppointmentViewModel>()
                });
            }
        }
        public async Task<IActionResult> Appointments()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (CurrentUser?.Role != "Doctor")
            {
                TempData["Error"] = "Access denied. This page is for doctors only.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = GetCurrentUserId();

                // Get doctor profile to get doctor ID
                var profile = await _api.GetAsync<DoctorProfileDto>($"/api/doctors/profile/{userId}");
                if (profile == null)
                {
                    TempData["Error"] = "Doctor profile not found.";
                    return RedirectToAction("Dashboard");
                }

                // Fetch all appointments for the doctor
                var appointments = await _api.GetAsync<List<DoctorAppointmentViewModel>>(
                    $"/api/doctors/{profile.Id}/appointments");

                return View(appointments ?? new List<DoctorAppointmentViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading appointments: " + ex.Message;
                return View(new List<DoctorAppointmentViewModel>());
            }
        }
    }

    // DTO for doctor profile API response
    public class DoctorProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string ClinicAddress { get; set; }
        public TimeSpan WorkStart { get; set; }
        public TimeSpan WorkEnd { get; set; }
        public string Bio { get; set; }
    }
}

