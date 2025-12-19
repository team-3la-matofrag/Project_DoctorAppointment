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

        public async Task<bool> IsAvailableAsync(
            int doctorId,
            DateTime startAt,
            DateTime endAt,
            int? excludeId = null)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                (excludeId == null || a.Id != excludeId) &&
                a.Status != "Canceled" &&
                a.Status != "Completed" &&
                ((startAt >= a.StartAt && startAt < a.EndAt) ||
                 (endAt > a.StartAt && endAt <= a.EndAt) ||
                 (startAt <= a.StartAt && endAt >= a.EndAt)));
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public Task<Appointment?> GetByIdAsync(int id)
        {
            return _context.Appointments.FindAsync(id).AsTask();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

      
        public async Task<List<object>> GetMyAppointmentsAsync(int patientId)
        {
            var now = DateTime.UtcNow;

            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartAt)
                .Select(a => new
                {
                    a.Id,
                    a.StartAt,
                    a.EndAt,
                    a.Status,
                    a.Notes,
                    DoctorName = a.Doctor.User.FullName,
                    IsUpcoming = a.StartAt >= now
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<object>> GetTodayAppointmentsAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId &&
                            a.StartAt >= today &&
                            a.StartAt < tomorrow)
                .OrderBy(a => a.StartAt)
                .Select(a => new
                {
                    a.Id,
                    a.StartAt,
                    a.EndAt,
                    a.Status,
                    a.Notes,
                    PatientName = a.Patient.User.FullName
                })
                .Cast<object>()
                .ToListAsync();
        }
    }
}
