using Project.DAL.Data;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class DoctorAvailabilityRepository
    {
        private readonly AppDbContext _context;

        public DoctorAvailabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<DoctorAvailability> GetByDoctorId(int doctorId)
        {
            return _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId)
                .ToList();
        }

        public void Add(DoctorAvailability availability)
        {
            _context.DoctorAvailabilities.Add(availability);
            _context.SaveChanges();
        }
    }
}
