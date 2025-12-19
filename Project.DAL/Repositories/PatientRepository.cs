using Project.DAL.Data;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class PatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public Patient GetById(int id)
        {
            return _context.Patients.Find(id);
        }
    }
}
