using Microsoft.AspNetCore.Mvc;
using Project.BLL.Interfaces;
using Project.BLL.Services;
using Project.DAL.Models;
using Project.MVC.Services;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class PatientController : BaseController
    {
        private readonly ApiService _api;

        public PatientController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");
            
            try
            {
                var userId = GetCurrentUserId();
                
                // Fetch dashboard data from API
                var dashboardData = await _api.GetAsync<Models.PatientDashboardViewModel>(
                    $"/api/patients/dashboard/{userId}");

                if (dashboardData == null)
                {
                    TempData["Warning"] = "Unable to load dashboard data. Please complete your profile.";
                    return RedirectToAction("Profile");
                }

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                
                // Return empty view model if error occurs
                return View(new Models.PatientDashboardViewModel
                {
                    Profile = new Models.PatientProfileViewModel 
                    { 
                        FullName = CurrentUser?.FullName ?? "Guest" 
                    },
                    Stats = new Models.DashboardStatsViewModel(),
                    UpcomingAppointments = new List<Models.UpcomingAppointmentViewModel>()
                });
            }
        }

        public async Task<IActionResult> Appointments()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            try
            {
                var currentUser = CurrentUser;
                Console.WriteLine($"DEBUG: CurrentUser is null?? {currentUser == null }"); //debugging
                Console.WriteLine($"DEBUG: UserId {currentUser.UserId}");//debugging
                var userId = GetCurrentUserId();
                Console.WriteLine($"DEBUG: GetCurrentUserId returns: {userId}");//debugging
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

        public async Task<IActionResult> Profile()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            try
            {
                var userId = GetCurrentUserId();
                var patient = await _api.GetAsync<Project.BLL.DTOs.PatientDto>($"/api/patients/profile/{userId}");

                if (patient == null)
                {
                    TempData["Warning"] = "Patient profile not found. Please contact support.";
                    return RedirectToAction("Dashboard");
                }

                var model = new Models.UpdatePatientProfileViewModel
                {
                    Id = patient.Id,
                    FullName = patient.FullName,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    DateOfBirth = patient.DOB,
                    Gender = patient.Gender,
                    Notes = patient.Notes
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Profile(Models.UpdatePatientProfileViewModel model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                var patient = await _api.GetAsync<Project.BLL.DTOs.PatientDto>($"/api/patients/profile/{userId}");

                if (patient == null)
                {
                    TempData["Error"] = "Patient profile not found.";
                    return RedirectToAction("Dashboard");
                }

                // Update patient data
                var updateDto = new Project.BLL.DTOs.PatientDto
                {
                    Id = patient.Id,
                    UserId = patient.UserId,
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    DOB = model.DateOfBirth,
                    Gender = model.Gender,
                    Notes = model.Notes ?? ""
                };

                await _api.PutAsync($"/api/patients/{patient.Id}", updateDto);

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update profile: " + ex.Message;
                return View(model);
            }
        }
    }
}
