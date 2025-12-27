namespace Project.MVC.Models
{
    public class PatientAppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public string DoctorName { get; set; }
        public bool IsUpcoming { get; set; }
    }
}
