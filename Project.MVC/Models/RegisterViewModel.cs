using System.ComponentModel.DataAnnotations;

namespace Project.MVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string? ErrorMessage { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        
        public bool IsDoctor { get; set; }
        public string Role => IsDoctor ? "Doctor" : "Patient";
        public string? Specialization { get; set; }
        public string? ClinicAddress { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan? WorkStart { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? WorkEnd { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
