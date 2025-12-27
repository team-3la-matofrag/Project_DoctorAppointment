using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Migrations;
using Project.DAL.Models;
using Project.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorService(IDoctorRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<DoctorDto>> GetAllAsync()
        {
            var doctors = await _repo.GetAllAsync();
            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                Specialization = d.Specialization.Name,
                IsActive = d.User.IsActive,
               WorkStart =d.WorkStart.ToString(),
               WorkEnd=d.WorkEnd.ToString(),
               ClinicAddress=d.ClinicAddress

            }).ToList();
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            return new DoctorDto
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                Specialization = d.Specialization.Name,
                IsActive = d.User.IsActive,
                 WorkStart = d.WorkStart.ToString(),
                WorkEnd = d.WorkEnd.ToString(),
                ClinicAddress = d.ClinicAddress
            };
        }

        public async Task AddAsync(CreateDoctorDto dto)
        {
          

            if (TimeSpan.Parse(dto.WorkEnd) <= TimeSpan.Parse(dto.WorkStart))
                throw new Exception("Invalid working hours");

            var doctor = new Doctor
            {
                UserId = dto.UserId,
                SpecializationId = dto.SpecializationId,
                ClinicAddress = dto.ClinicAddress,
                WorkStart = TimeSpan.Parse(dto.WorkStart),
                WorkEnd = TimeSpan.Parse(dto.WorkEnd)
            };

            await _repo.AddAsync(doctor);
        }


        public async Task UpdateAsync(int id, DoctorDto dto)
        {
            var doctor = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Doctor not found");

            doctor.User.IsActive = dto.IsActive;
            doctor.ClinicAddress = dto.ClinicAddress;
            doctor.WorkStart = TimeSpan.Parse(dto.WorkStart);
            doctor.WorkEnd = TimeSpan.Parse(dto.WorkEnd);
            doctor.User.FullName = dto.FullName;
            doctor.User.Email = dto.Email;
            
            await _repo.SaveChangesAsync();
        }


        public async Task ToggleStatusAsync(int id)
        {
            var doctor = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Doctor not found");

            doctor.User.IsActive = !doctor.User.IsActive;
            await _repo.SaveChangesAsync();
        }
    }
}
