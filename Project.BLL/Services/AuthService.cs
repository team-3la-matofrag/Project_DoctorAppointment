using Microsoft.AspNetCore.Identity;
using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Repositories;
using Project.DAL.Models;

namespace Project.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly SpecializationRepository _specializationRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IPatientRepository patientRepository, SpecializationRepository specializationRepository, IDoctorRepository doctorRepository)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _specializationRepository = specializationRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<object> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new Exception("Account is disabled");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid email or password");
            }

            return new
            {
                UserId = user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Phone
            };
        }

        public async Task<object> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            if (string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot self-register as Admin");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = email,
                Role = !string.IsNullOrEmpty(dto.Role) ? dto.Role : "Patient",
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            if(user.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    DOB = dto.DateOfBirth,
                    Gender =dto.Gender,
                    Notes = string.Empty
                };
                await _patientRepository.AddAsync(patient);
                await _patientRepository.SaveChangesAsync();
            }else if(user.Role == "Doctor")
            {
                int? specializationId = null;
                if (!string.IsNullOrWhiteSpace(dto.Specialization))
                {
                    var specializations = _specializationRepository.GetAll();
                    var specialization = specializations
                        .FirstOrDefault(s => s.Name.Equals(dto.Specialization, StringComparison.OrdinalIgnoreCase));

                    if (specialization == null)
                    {
                        specialization = new Specialization { Name = dto.Specialization };
                        _specializationRepository.Add(specialization);
                        specializations = _specializationRepository.GetAll(); // Refresh
                        specialization = specializations.First(s => s.Name == dto.Specialization);
                    }

                    specializationId = specialization.Id;
                }

                var doctor = new Doctor
                {
                    UserId = user.Id,
                    SpecializationId = specializationId,
                    Bio = dto.DoctorNotes ?? string.Empty,
                    ClinicAddress = dto.ClinicAddress ?? string.Empty,
                    WorkStart = dto.WorkStart ?? new TimeSpan(9, 0, 0), // Default 9 AM
                    WorkEnd = dto.WorkEnd ?? new TimeSpan(17, 0, 0) // Default 5 PM
                };

                await _doctorRepository.AddAsync(doctor);
                await _doctorRepository.SaveChangesAsync();
            }
        
            return new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Phone
            };
        }
    }
}
