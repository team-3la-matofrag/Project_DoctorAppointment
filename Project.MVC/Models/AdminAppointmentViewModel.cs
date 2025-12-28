namespace Project.MVC.Models
{
    public class AdminAppointmentViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; } 
        public DateTime Date { get; set; }

        public string DoctorName { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string Notes { get; set; }
    }
}
