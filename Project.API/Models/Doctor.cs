namespace Project.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? SpecializationId { get; set; }
        public Specialization Specialization { get; set; }

        public string Bio { get; set; }
        public string ClinicAddress { get; set; }

        public ICollection<DoctorAvailability> Availabilities { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
