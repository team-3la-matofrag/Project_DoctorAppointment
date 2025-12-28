using Project.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto?> GetByIdAsync(int id);
        Task<PatientDto?> GetProfileAsync(int userId);
        Task<List<PatientDto>> GetAllAsync();
        Task<PatientDto> CreateAsync(PatientDto dto);
        Task UpdateAsync(int id,PatientDto dto);
        Task DeleteAsync(int id);
        Task<List<object>> GetPatientAppointmentsAsync(int patientId);
        Task<PatientDashboardDto> GetDashboardDataAsync(int userId);

    }
}
