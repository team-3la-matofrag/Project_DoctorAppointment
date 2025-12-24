using Microsoft.AspNetCore.Identity;
using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                user.Id,
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
                Role = dto.Role,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

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
