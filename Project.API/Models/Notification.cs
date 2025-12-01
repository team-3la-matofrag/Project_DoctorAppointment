namespace Project.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public string Channel { get; set; }      // Email / SMS / System
        public DateTime ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string Status { get; set; }       // Pending / Sent / Failed
        public object CreatedAt { get; internal set; }
        public bool IsRead { get; internal set; }
    }
}
