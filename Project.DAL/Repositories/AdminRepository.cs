using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;
using Project.DAL.Interfaces;
using Project.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetDoctorsCountAsync()
        {
            return await _context.Doctors.CountAsync();
        }

        public async Task<int> GetPatientsCountAsync()
        {
            return await _context.Patients.CountAsync();
        }

        public async Task<int> GetAppointmentsCountAsync()
        {
            return await _context.Appointments.CountAsync();
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();
        }

        public async Task ActivateDoctorAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor != null)
            {
                doctor.User.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ForceCancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled; 
            await _context.SaveChangesAsync();
        }
    }
}
