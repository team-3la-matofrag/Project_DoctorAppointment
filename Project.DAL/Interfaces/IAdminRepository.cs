using Project.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.DAL.Interfaces
{
    public interface IAdminRepository
    {
        Task<int> GetDoctorsCountAsync();
        Task<int> GetPatientsCountAsync();
        Task<int> GetAppointmentsCountAsync();

        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<List<Patient>> GetAllPatientsAsync();
        Task<List<Appointment>> GetAllAppointmentsAsync();

        Task ActivateDoctorAsync(int doctorId);
        Task ForceCancelAppointmentAsync(int appointmentId);
    }
}
