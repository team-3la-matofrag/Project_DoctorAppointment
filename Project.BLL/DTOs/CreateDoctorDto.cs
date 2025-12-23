using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.DTOs
{
    public class CreateDoctorDto
    {
        public int UserId { get; set; }
        public int SpecializationId { get; set; }
        public string WorkStart { get;  set; }
        public string WorkEnd { get;  set; }
        public string ClinicAddress { get;  set; }
        public bool IsActive { get;  set; }
    }

}
