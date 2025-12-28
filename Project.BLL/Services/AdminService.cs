using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            return new AdminDashboardDto
            {
                DoctorsCount = await _adminRepository.GetDoctorsCountAsync(),
                PatientsCount = await _adminRepository.GetPatientsCountAsync(),
                AppointmentsCount = await _adminRepository.GetAppointmentsCountAsync()
            };
        }

        public async Task<List<AdminDoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _adminRepository.GetAllDoctorsAsync();

            return doctors
                .Where(d => d.User != null)
                .Select(d => new AdminDoctorDto
                {
                    Id = d.Id,
                    FullName = d.User!.FullName,
                    IsActive = d.User.IsActive
                })
                .ToList();
        }
        public async Task ActivateDoctorAsync(int doctorId)
        {
            await _adminRepository.ActivateDoctorAsync(doctorId);
        }

        public async Task<List<AdminPatientDto>> GetAllPatientsAsync()
        {
            var patients = await _adminRepository.GetAllPatientsAsync();

            return patients.Select(p => new AdminPatientDto
            {
                Id = p.Id,
                FullName = p.User.FullName
            }).ToList();
        }

        public async Task<List<AdminAppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _adminRepository.GetAllAppointmentsAsync();

            return appointments.Select(a => new AdminAppointmentDto
            {
                Id = a.Id,
                DoctorName = a.Doctor?.User?.FullName ?? "N/A",
                PatientName = a.Patient?.User?.FullName ?? "N/A",
                Status = a.Status
            }).ToList();
        }

        public async Task ForceCancelAppointmentAsync(int appointmentId)
        {
            await _adminRepository.ForceCancelAppointmentAsync(appointmentId);
        }
    }
}
