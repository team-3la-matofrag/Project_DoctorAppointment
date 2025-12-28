using System.ComponentModel.DataAnnotations;

namespace Project.MVC.Models
{
    public class BookAppointmentViewModel
    {
        [Required(ErrorMessage = "Please select a specialty")]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Please select a time slot")]
        public string TimeSlot { get; set; }

        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string? Reason { get; set; }

        // Helper properties for display
        public List<string>? AvailableSpecialties { get; set; }
        public List<DoctorOption>? AvailableDoctors { get; set; }
        public List<string>? AvailableTimeSlots { get; set; }
    }

    public class DoctorOption
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
    }

    public class TimeSlotAvailability
    {
        public string TimeSlot { get; set; }
        public bool IsAvailable { get; set; }
    }
}
