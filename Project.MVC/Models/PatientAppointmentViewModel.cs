namespace Project.MVC.Models
{
    public class PatientAppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public string DoctorName { get; set; }
        public string? DoctorSpecialty { get; set; }
        public string? Location { get; set; }
        public bool IsUpcoming { get; set; }

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

        public string StatusIcon
        {
            get
            {
                return Status?.ToLower() switch
                {
                    "confirmed" => "bi-check-circle-fill",
                    "pending" => "bi-clock-fill",
                    "completed" => "bi-check-all",
                    "cancelled" => "bi-x-circle-fill",
                    _ => "bi-question-circle-fill"
                };
            }
        }

        public bool CanCancel => Status?.ToLower() == "pending" || Status?.ToLower() == "confirmed";
        public bool IsPast => StartAt < DateTime.Now;
    }
}

