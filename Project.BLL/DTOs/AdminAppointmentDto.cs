using Project.DAL.Models;

namespace Project.BLL.DTOs
{
    public class AdminAppointmentDto
    {
        public int Id { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
