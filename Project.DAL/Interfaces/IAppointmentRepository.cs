using Project.DAL.Models;

namespace Project.DAL.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> IsAvailableAsync(
            int doctorId,
            DateTime startAt,
            DateTime endAt,
            int? excludeId = null);

        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment?> GetByIdAsync(int id);
        Task SaveChangesAsync();

      
        Task<List<object>> GetMyAppointmentsAsync(int patientId);
        Task<List<object>> GetTodayAppointmentsAsync(int doctorId);
    }
}
