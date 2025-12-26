using Project.DAL.Models;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(int id);
    Task<List<Appointment>> GetByDoctorAsync(int doctorId);
    Task<List<Appointment>> GetByPatientAsync(int patientId);
    Task<List<Appointment>> GetAllWithUsersAsync();

    Task<bool> HasConflictAsync(int doctorId, DateTime start, DateTime end);
    Task SaveChangesAsync();
}
