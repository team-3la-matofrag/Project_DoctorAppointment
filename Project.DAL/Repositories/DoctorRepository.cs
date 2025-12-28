using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;
using Project.DAL.Interfaces;
using Project.DAL.Models;

public class DoctorRepository : IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .ToListAsync();
    }
    public async Task<Doctor?> GetByUserIdWithAppointmentsAsync(int userId)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .Include(d => d.Appointments)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialization)
            .Include(d => d.Appointments)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
