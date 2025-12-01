namespace Project.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public string Status { get; set; }  // Pending / Confirmed / Completed / Canceled
        public string Notes { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
