using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public Task<Appointment?> GetByIdAsync(int id)
            => _context.Appointments.FindAsync(id).AsTask();

        public Task<List<Appointment>> GetByDoctorAsync(int doctorId)
            => _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.StartAt)
                .ToListAsync();

        public Task<List<Appointment>> GetByPatientAsync(int patientId)
            => _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();

        public async Task<bool> HasConflictAsync(
            int doctorId,
            DateTime start,
            DateTime end)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.Status != AppointmentStatus.Cancelled &&
                (
                    (start >= a.StartAt && start < a.EndAt) ||
                    (end > a.StartAt && end <= a.EndAt) ||
                    (start <= a.StartAt && end >= a.EndAt)
                ));
        }
        public async Task<List<Appointment>> GetAllWithUsersAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        public Task SaveChangesAsync()
            => _context.SaveChangesAsync();


    }

}
