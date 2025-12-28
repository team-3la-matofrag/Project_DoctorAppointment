using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Migrations;
using Project.DAL.Models;
using Project.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorService(IDoctorRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<DoctorDto>> GetAllAsync()
        {
            var doctors = await _repo.GetAllAsync();
            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                Specialization = d.Specialization.Name,
                IsActive = d.User.IsActive,
               WorkStart =d.WorkStart.ToString(),
               WorkEnd=d.WorkEnd.ToString(),
               ClinicAddress=d.ClinicAddress

            }).ToList();
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            return new DoctorDto
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                Specialization = d.Specialization.Name,
                IsActive = d.User.IsActive,
                 WorkStart = d.WorkStart.ToString(),
                WorkEnd = d.WorkEnd.ToString(),
                ClinicAddress = d.ClinicAddress
            };
        }

        public async Task AddAsync(CreateDoctorDto dto)
        {
          

            if (TimeSpan.Parse(dto.WorkEnd) <= TimeSpan.Parse(dto.WorkStart))
                throw new Exception("Invalid working hours");

            var doctor = new Doctor
            {
                UserId = dto.UserId,
                SpecializationId = dto.SpecializationId,
                ClinicAddress = dto.ClinicAddress,
                WorkStart = TimeSpan.Parse(dto.WorkStart),
                WorkEnd = TimeSpan.Parse(dto.WorkEnd)
            };

            await _repo.AddAsync(doctor);
        }


        public async Task UpdateAsync(int id, DoctorDto dto)
        {
            var doctor = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Doctor not found");

            doctor.User.IsActive = dto.IsActive;
            doctor.ClinicAddress = dto.ClinicAddress;
            doctor.WorkStart = TimeSpan.Parse(dto.WorkStart);
            doctor.WorkEnd = TimeSpan.Parse(dto.WorkEnd);
            doctor.User.FullName = dto.FullName;
            doctor.User.Email = dto.Email;
            
            await _repo.SaveChangesAsync();
        }


        public async Task ToggleStatusAsync(int id)
        {
            var doctor = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Doctor not found");

            doctor.User.IsActive = !doctor.User.IsActive;
            await _repo.SaveChangesAsync();
        }
        public async Task<object> GetDashboardDataAsync(int userId)
        {
            var doctor =  await _repo.GetByUserIdWithAppointmentsAsync(userId);
            if (doctor == null) throw new Exception("Doctor Profile Not found");

            var now = DateTime.UtcNow;
            var today = DateTime.Today;
            var todayEnd = today.AddDays(1);

            var allAppointments = doctor.Appointments?.ToList() ?? new List<Appointment>();
            var todayAppointments = allAppointments
                .Where(a => a.StartAt >= today && a.EndAt < todayEnd)
                .OrderBy(a => a.StartAt)
                .ToList();
            var todayTotal = todayAppointments.Count;
            var todayPending = todayAppointments.Count(a => a.Status == AppointmentStatus.Pending);
            var todayConfirmed = todayAppointments.Count(a => a.Status == AppointmentStatus.Confirmed);

            var uniquePatients = allAppointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .Select(a => a.PatientId)
                .Distinct()
                .Count();
            var upcomingAppointments = allAppointments
                .Count(a => a.StartAt >= now && a.Status != AppointmentStatus.Cancelled);
            return new
            {
                Profile = new
                {
                    Id = doctor.Id,
                    FullName = doctor.User?.FullName ?? "",
                    Email = doctor.User?.Email ?? "",
                    Specialization = doctor.Specialization?.Name ?? "General",
                    ClinicAddress = doctor.ClinicAddress ?? "",
                    WorkingHours = $"{doctor.WorkStart:hh\\:mm} - {doctor.WorkEnd:hh\\:mm}"
                },
                Stats = new
                {
                    TodayTotal = todayTotal,
                    TodayPending = todayPending,
                    TodayConfirmed = todayConfirmed,
                    TotalPatients = uniquePatients,
                    UpcomingAppointments = upcomingAppointments
                },
                TodayAppointments = todayAppointments.Select(a => new
                {
                    a.Id,
                    a.StartAt,
                    a.EndAt,
                    Status = a.Status.ToString(),
                    a.Notes,
                    PatientName = a.Patient?.User?.FullName ?? "Unknown",
                    PatientPhone = a.Patient?.User?.Phone ?? ""
                }).ToList()
            };
        }
        public async Task<object> GetProfileAsync(int userId)
        {
            var doctors = await _repo.GetAllAsync();
            var doctor = doctors.FirstOrDefault(d => d.UserId == userId);

            if (doctor == null) return null;

            return new
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.User?.FullName ?? "",
                Email = doctor.User?.Email ?? "",
                Phone = doctor.User?.Phone ?? "",
                Specialization = doctor.Specialization?.Name ?? "",
                ClinicAddress = doctor.ClinicAddress ?? " ",
                WorkStart = doctor.WorkStart,
                WorkEnd = doctor.WorkEnd,
                Bio = doctor.Bio ?? ""
            };
        }
        public async Task<List<object>> GetDoctorAppointmentsAsync(int doctorId, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var doctor = await _repo.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            var appointments = doctor.Appointments?.ToList() ?? new List<Appointment>();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<AppointmentStatus>(status, true, out var statusEnum))
                {
                    appointments = appointments.Where(a => a.Status == statusEnum).ToList();
                }
            }

            if (startDate.HasValue)
            {
                appointments = appointments.Where(a => a.StartAt.Date >= startDate.Value.Date).ToList();
            }

            if (endDate.HasValue)
            {
                appointments = appointments.Where(a => a.StartAt.Date <= endDate.Value.Date).ToList();
            }

            return appointments
                .OrderByDescending(a => a.StartAt)
                .Select(a => new
                {
                    a.Id,
                    a.StartAt,
                    a.EndAt,
                    Status = a.Status.ToString(),
                    a.Notes,
                    PatientName = a.Patient?.User?.FullName ?? "Unknown",
                    PatientPhone = a.Patient?.User?.Phone ?? "",
                    PatientEmail = a.Patient?.User?.Email ?? "",
                    PatientGender = a.Patient?.Gender ?? "",
                    PatientAge = a.Patient?.DOB != null
                        ? DateTime.Today.Year - a.Patient.DOB.Year
                        : 0
                })
                .Cast<object>()
                .ToList();

        }
    }
}
