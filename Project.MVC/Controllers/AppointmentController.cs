using Microsoft.AspNetCore.Mvc;
using Project.BLL.DTOs;
using Project.DAL.Models;
using Project.MVC.Services;
using Project.MVC.Models;

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
        public async Task<IActionResult> Book()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var model = new BookAppointmentViewModel
            {
                AppointmentDate = DateTime.Today.AddDays(1), // Default to tomorrow
                AvailableSpecialties = await GetSpecialtiesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Book(BookAppointmentViewModel model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.AvailableSpecialties = await GetSpecialtiesAsync();
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                // Find patient ID associated with user
                var patient = await _api.GetAsync<PatientDto>($"/api/patients/profile/{userId}"); 
                if (patient == null) 
                {
                    TempData["Error"] = "Patient profile not found. Please complete your profile first.";
                    return RedirectToAction("Profile", "Patient");
                }

                // Parse time slot to get hour and minute
                var timeParts = model.TimeSlot.Split(':');
                var appointmentDateTime = model.AppointmentDate.Date
                    .AddHours(int.Parse(timeParts[0]))
                    .AddMinutes(int.Parse(timeParts[1]));

                var dto = new CreateAppointmentDto
                {
                    DoctorId = model.DoctorId,
                    PatientId = patient.Id,
                    StartAt = appointmentDateTime,
                    EndAt = appointmentDateTime.AddMinutes(30), // Default 30-minute duration
                    Notes = model.Reason
                };

                await _api.PostAsync("/api/appointments", dto);
                
                TempData["Success"] = "Appointment booked successfully! You will receive a confirmation soon.";
                return RedirectToAction("Dashboard", "Patient");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to book appointment: " + ex.Message;
                model.AvailableSpecialties = await GetSpecialtiesAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpecialty(string specialty)
        {
            try
            {
                var doctors = await _api.GetAsync<List<DoctorDto>>("/api/doctors");
                var filtered = doctors
                    .Where(d => d.IsActive && string.Equals(d.Specialization, specialty, StringComparison.OrdinalIgnoreCase))
                    .Select(d => new { 
                        id = d.Id, 
                        fullName = d.FullName,
                        specialty = d.Specialization
                    })
                    .ToList();
                
                return Json(filtered);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int doctorId, string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime appointmentDate))
                {
                    return Json(new { error = "Invalid date format" });
                }

                // Get doctor's appointments for that day
                var doctorAppointments = await _api.GetAsync<List<object>>($"/api/appointments/doctor/{doctorId}");
                
                // Get doctor details for working hours
                var doctor = await _api.GetAsync<DoctorDto>($"/api/doctors/{doctorId}");
                
                // Generate time slots (e.g., 9:00 AM to 5:00 PM, 30-minute intervals)
                var timeSlots = GenerateTimeSlots(doctor?.WorkStart, doctor?.WorkEnd);
                
                // Mark occupied slots (simplified - in production, parse actual appointments)
                var availableSlots = timeSlots.Select(slot => new
                {
                    timeSlot = slot,
                    isAvailable = true // Simplified - should check against existing appointments
                }).ToList();

                return Json(availableSlots);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<List<string>> GetSpecialtiesAsync()
        {
            try
            {
                var doctors = await _api.GetAsync<List<DoctorDto>>("/api/doctors");
                return doctors
                    .Where(d => d.IsActive && !string.IsNullOrEmpty(d.Specialization))
                    .Select(d => d.Specialization)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
            }
            catch
            {
                // Fallback to hardcoded list
                return new List<string>
                {
                    "Cardiology",
                    "Dermatology",
                    "Neurology",
                    "Orthopedics",
                    "Pediatrics",
                    "General Medicine"
                };
            }
        }

        private List<string> GenerateTimeSlots(string? workStart, string? workEnd)
        {
            var slots = new List<string>();
            
            // Default working hours: 9:00 AM to 5:00 PM
            var startHour = 9;
            var endHour = 17;

            if (!string.IsNullOrEmpty(workStart) && TimeSpan.TryParse(workStart, out var start))
            {
                startHour = start.Hours;
            }

            if (!string.IsNullOrEmpty(workEnd) && TimeSpan.TryParse(workEnd, out var end))
            {
                endHour = end.Hours;
            }

            // Generate 30-minute intervals
            for (int hour = startHour; hour < endHour; hour++)
            {
                slots.Add($"{hour:D2}:00");
                slots.Add($"{hour:D2}:30");
            }

            return slots;
        }

        public IActionResult MyAppointments()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            int userId = GetCurrentUserId();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");
            try
            {
                await _api.PostAsync($"/api/appointments/{id}/cancel", new { });
                TempData["Success"] = "Appointment cancelled successfully.";
                return RedirectToAction("Appointments", "Patient");
            }
            catch (Exception ex)
            {
                TempData["Error"]= "Failed to cancel appointment: " + ex.Message;
                return RedirectToAction("Appointments", "Patient");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            if (!IsAuthenticated())
            {
                TempData["Error"] = "Please log in to confirm appointments.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _api.PostAsync($"/api/appointments/{id}/confirm", new { });
                
                TempData["Success"] = "Appointment confirmed successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to confirm appointment: " + ex.Message;
            }

            // Redirect based on user role
            if (CurrentUser?.Role == "Doctor")
                return RedirectToAction("Appointments", "Doctor");
            else
                return RedirectToAction("Appointments", "Patient");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            if (!IsAuthenticated())
            {
                TempData["Error"] = "Please log in to complete appointments.";
                return RedirectToAction("Login", "Account");
            }

            // Only doctors can complete appointments
            if (CurrentUser?.Role != "Doctor")
            {
                TempData["Error"] = "Only doctors can mark appointments as completed.";
                return RedirectToAction("Dashboard", "Patient");
            }

            try
            {
                await _api.PutAsync($"/api/appointments/{id}/complete", new { });
                
                TempData["Success"] = "Appointment marked as completed!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to complete appointment: " + ex.Message;
            }

            return RedirectToAction("Appointments", "Doctor");
        }
    }
}
