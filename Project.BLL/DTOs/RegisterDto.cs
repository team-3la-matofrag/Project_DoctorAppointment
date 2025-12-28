using System.ComponentModel.DataAnnotations;

namespace Project.BLL.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; } = "Patient";
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        // Doctor specific fields 
        public string? Specialization { get; set; }
        public string? ClinicAddress { get; set; }
        public TimeSpan? WorkStart { get; set; }
        public TimeSpan? WorkEnd { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
