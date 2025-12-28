namespace Project.BLL.DTOs
{
    public class PatientDashboardDto
    {
        public PatientProfileDto Profile { get; set; }
        public DashboardStatsDto Stats { get; set; }
        public List<UpcomingAppointmentDto> UpcomingAppointments { get; set; }
    }

    public class PatientProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int DoctorsVisited { get; set; }
    }

    public class UpcomingAppointmentDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public bool IsNext { get; set; }
    }
}
