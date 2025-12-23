using Project.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByUserIdAsync(int Userid);
        Task<List<Patient>> GetAllAsync();
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);
        Task<List<Appointment>> GetPatientAppointmentsAsync(int patientId);
        Task SaveChangesAsync();

    }
}
