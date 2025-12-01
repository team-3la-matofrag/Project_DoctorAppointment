namespace Project.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Notes { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
