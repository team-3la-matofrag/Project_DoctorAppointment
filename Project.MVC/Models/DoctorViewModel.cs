namespace Project.MVC.Models
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Specialty { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; } 
        public string? Qualifications { get; set; }
        public string? WorkHours { get; set; }
    }
}