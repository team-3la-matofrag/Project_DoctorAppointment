using Project.DAL.Models;
using Project.BLL.DTOs;

namespace Project.BLL.Interfaces
{
    public interface IAppointmentService
    {
        Task<bool> CheckAvailabilityAsync(int doctorId, DateTime startAt, DateTime endAt);

        Task<Appointment> AddAppointmentAsync(AppointmentDto dto);
        Task CancelAsync(int appointmentId);
        Task ConfirmAsync(int appointmentId);

        Task RescheduleAsync(
            int appointmentId,
            DateTime startAt,
            DateTime endAt,
            string? notes);

        Task<List<object>> GetMyAppointmentsAsync(int patientId);
        Task<List<object>> GetTodayAppointmentsAsync(int doctorId);
    }
}
