using Project.DAL.Data;
using Project.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Project.DAL.Repositories
{
    public class DoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Doctor> GetAll()
        {
            return _context.Doctors
                .Include(d => d.Specialization)
                .ToList();
        }

        public Doctor GetById(int id)
        {
            return _context.Doctors.Find(id);
        }

        public void Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }
    }
}
