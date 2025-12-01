using System.Numerics;

namespace Project.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Patient / Doctor / Admin
        public string Role { get; set; }

        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
