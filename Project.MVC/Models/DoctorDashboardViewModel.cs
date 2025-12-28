namespace Project.MVC.Models
{
    public class DoctorDashboardViewModel
    {
        public DoctorProfileViewModel Profile { get; set; }
        public DoctorStatsViewModel Stats { get; set; }
        public List<DoctorAppointmentViewModel> TodayAppointments { get; set; }
    }
    public class DoctorStatsViewModel
    {
        public int TodayTotal { get; set; }
        public int TodayPending { get; set; }
        public int TodayConfirmed { get; set; }
        public int TotalPatients { get; set; }
        public int UpcomingAppointments { get; set; }

    }
    public class DoctorProfileViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string ClinicAddress { get; set; }
        public string WorkingHours { get; set; }

    }
    public class DoctorAppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public string PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? PatientEmail { get; set; }
        public string? PatientGender { get; set; }
        public int PatientAge { get; set; }

        // Helper properties for display
        public string FormattedDate => StartAt.ToString("MMM dd, yyyy");
        public string FormattedTime => StartAt.ToString("h:mm tt");
        public string FormattedDuration => $"{(EndAt - StartAt).TotalMinutes} min";

        public string RelativeDate
        {
            get
            {
                var today = DateTime.Today;
                var appointmentDay = StartAt.Date;

                if (appointmentDay == today)
                    return "Today";
                if (appointmentDay == today.AddDays(1))
                    return "Tomorrow";
                if (appointmentDay == today.AddDays(-1))
                    return "Yesterday";
                if (appointmentDay > today && appointmentDay <= today.AddDays(7))
                    return appointmentDay.ToString("dddd");
                return FormattedDate;
            }
        }

        public string StatusColor => Status?.ToLower() switch
        {
            "confirmed" => "success",
            "pending" => "warning",
            "completed" => "info",
            "cancelled" => "danger",
            _ => "secondary"
        };

        public string StatusIcon => Status?.ToLower() switch
        {
            "confirmed" => "bi-check-circle-fill",
            "pending" => "bi-clock-fill",
            "completed" => "bi-check-all",
            "cancelled" => "bi-x-circle-fill",
            _ => "bi-question-circle-fill"
        };

        public bool CanConfirm => Status?.ToLower() == "pending";
        public bool CanComplete => Status?.ToLower() == "confirmed";
        public bool CanCancel => Status?.ToLower() == "pending" || Status?.ToLower() == "confirmed";
        public bool IsPast => StartAt < DateTime.Now;
        public bool IsUpcoming => StartAt >= DateTime.Now && Status?.ToLower() != "cancelled";
    }

}
