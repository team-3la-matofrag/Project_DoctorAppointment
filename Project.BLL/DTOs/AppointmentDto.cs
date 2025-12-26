namespace Project.BLL.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
    }

}
