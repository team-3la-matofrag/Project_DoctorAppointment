using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;

        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if ( patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            return await _context.Patients
                   .Include(p => p.User)
                   .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient?> GetByUserIdAsync(int userid)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userid);
        }

        public async Task<List<Appointment>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Update(patient);
            await _context.SaveChangesAsync();
        }
    }
}
