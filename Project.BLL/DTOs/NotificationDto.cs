using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.DTOs
{

    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Channel { get; set; }  = "System";
        public string Status { get; set; }  = "Pending";
        public DateTime ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
    }
}



