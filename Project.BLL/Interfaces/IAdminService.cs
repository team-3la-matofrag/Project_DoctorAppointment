using Project.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();

        Task<List<AdminDoctorDto>> GetAllDoctorsAsync();
        Task ActivateDoctorAsync(int doctorId);

        Task<List<AdminPatientDto>> GetAllPatientsAsync();

        Task<List<AdminAppointmentDto>> GetAllAppointmentsAsync();
        Task ForceCancelAppointmentAsync(int appointmentId);
    }
}
