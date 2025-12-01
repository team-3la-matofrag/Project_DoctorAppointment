namespace Project.Models
{
    public class DoctorAvailability
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public string DayOfWeek { get; set; }  
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int SlotMinutes { get; set; }
    }
}
