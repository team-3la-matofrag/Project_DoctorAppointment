using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Specialization { get; set; } = null!;
       

        public string? ClinicAddress { get; set; }
        public string? WorkStart { get; set; }
        public string? WorkEnd { get; set; }
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
    }

}
