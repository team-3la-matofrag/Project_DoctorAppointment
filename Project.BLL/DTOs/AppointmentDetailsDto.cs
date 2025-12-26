using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.DTOs
{
    public class AppointmentDetailsDto
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }
    }

}
