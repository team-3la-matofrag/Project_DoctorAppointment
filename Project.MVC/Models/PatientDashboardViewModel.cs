namespace Project.MVC.Models
{
    public class PatientDashboardViewModel
    {
        public PatientProfileViewModel Profile { get; set; }
        public DashboardStatsViewModel Stats { get; set; }
        public List<UpcomingAppointmentViewModel> UpcomingAppointments { get; set; }
    }

    public class PatientProfileViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }

    public class DashboardStatsViewModel
    {
        public int TotalAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int DoctorsVisited { get; set; }
    }

    public class UpcomingAppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public bool IsNext { get; set; }

        // Helper properties for display
        public string FormattedDate => AppointmentDate.ToString("MMM dd, yyyy");
        public string FormattedTime => AppointmentTime.ToString("h:mm tt");
        public string RelativeDate
        {
            get
            {
                var today = DateTime.Today;
                var appointmentDay = AppointmentDate.Date;

                if (appointmentDay == today)
                    return "Today";
                if (appointmentDay == today.AddDays(1))
                    return "Tomorrow";
                if (appointmentDay > today && appointmentDay <= today.AddDays(7))
                    return appointmentDay.ToString("dddd"); // Day name
                return FormattedDate;
            }
        }

        public string StatusColor
        {
            get
            {
                return Status?.ToLower() switch
                {
                    "confirmed" => "success",
                    "pending" => "warning",
                    "completed" => "info",
                    "cancelled" => "danger",
                    _ => "secondary"
                };
            }
        }

        public string SpecialtyIcon
        {
            get
            {
                return Specialty?.ToLower() switch
                {
                    "cardiology" => "bi-heart-pulse",
                    "dermatology" => "bi-bandaid",
                    "neurology" => "bi-brain",
                    "orthopedics" => "bi-capsule",
                    "pediatrics" => "bi-person",
                    "ophthalmology" => "bi-eye",
                    "dentistry" => "bi-chat-heart",
                    _ => "bi-hospital"
                };
            }
        }
    }
}
