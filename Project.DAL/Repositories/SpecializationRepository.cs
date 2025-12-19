using Project.DAL.Data;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class SpecializationRepository
    {
        private readonly AppDbContext _context;

        public SpecializationRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Specialization> GetAll()
        {
            return _context.Specializations.ToList();
        }

        public void Add(Specialization specialization)
        {
            _context.Specializations.Add(specialization);
            _context.SaveChanges();
        }
    }
}
