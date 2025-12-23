using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.BLL.Services
{
    public class PatientService : IPatientService
    {
        public readonly IPatientRepository _repo;
        public PatientService(IPatientRepository repo)
        {
            _repo = repo;
        }
        public async Task<PatientDto> CreateAsync(PatientDto dto)
        {
            var patient = new Patient
            {
                UserId = dto.UserId,
                DOB = dto.DOB,
                Gender = dto.Gender,
                Notes = dto.Notes ?? ""
            };
            var created = await _repo.AddAsync(patient);

            return new PatientDto
            {
                Id = created.Id,
                UserId = created.UserId,
                DOB = created.DOB,
                Gender = created.Gender,
                Notes = created.Notes
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<List<PatientDto>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();
            return patients.Select(p => new PatientDto
            {
                Id = p.Id,
                UserId = p.UserId,
                FullName = p.User?.FullName ?? "",
                Email = p.User?.Email ?? "",
                DOB = p.DOB,
                Gender = p.Gender,
                Notes = p.Notes
            }).ToList();
        }

        public async Task<PatientDto?> GetByIdAsync(int id)
        {
            var patient = await _repo.GetByIdAsync(id); 
            if(patient == null) return null;
            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                FullName = patient.User?.FullName ?? "",
                Email = patient.User?.Email ?? "",
                DOB = patient.DOB,
                Gender = patient.Gender,
                Notes = patient.Notes
            };
        }

        public async Task<PatientDto?> GetProfileAsync(int userId)
        {
            var patient = await _repo.GetByUserIdAsync(userId);
            if (patient == null) { return null; }
            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                FullName = patient.User?.FullName ?? "",
                Email = patient.User?.Email ?? "",
                DOB = patient.DOB,
                Gender = patient.Gender,
                Notes = patient.Notes
            };
        }

        public async Task<List<object>> GetPatientAppointmentsAsync(int patientId)
        {
            var appointments = await _repo.GetPatientAppointmentsAsync(patientId);
            var now = DateTime.UtcNow;
            return appointments.Select(a => new
            {
                a.Id,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.Notes,
                DoctorName = a.Doctor?.User?.FullName ?? "Unknown",
                IsUpcoming = a.StartAt >= now


            }
                ).Cast<object>().ToList();

        }

        public  async Task UpdateAsync(int id, PatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(id) ?? throw new Exception("Patient Not Found");
            patient.DOB = dto.DOB;
            patient.Gender = dto.Gender;
            patient.Notes = dto.Notes ?? "";

            await _repo.UpdateAsync(patient);
        }
    }
}
