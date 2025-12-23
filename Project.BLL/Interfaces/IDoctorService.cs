using Project.BLL.DTOs;
using Project.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.Interfaces
{
    public interface IDoctorService
    {
        Task<List<DoctorDto>> GetAllAsync();
        Task<DoctorDto?> GetByIdAsync(int id);
        
        

        Task UpdateAsync(int id, DoctorDto dto);
        Task AddAsync(CreateDoctorDto dto);

        Task ToggleStatusAsync(int id);
    }
}
