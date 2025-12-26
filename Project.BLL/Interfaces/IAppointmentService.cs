using Project.BLL.DTOs;

namespace Project.BLL.Interfaces
{
    public interface IAppointmentService
    {
        Task CreateAsync(CreateAppointmentDto dto);
        Task CancelAsync(int id);
        Task CompleteAsync(int appointmentId);
        Task ConfirmAsync(int appointmentId);
        Task<List<AppointmentDetailsDto>> GetAllAsync();



        Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
    }

}