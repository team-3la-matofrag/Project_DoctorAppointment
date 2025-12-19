using Project.DAL.Data;
using Project.DAL.Models;

namespace Project.DAL.Repositories
{
    public class NotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<Notification> GetByUserId(int userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .ToList();
        }
    }
}
