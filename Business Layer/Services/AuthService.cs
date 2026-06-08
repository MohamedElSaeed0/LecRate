using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfRate.Data;
using ProfRate.DTOs;
using ProfRate.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProfRate.Services
{
    
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        public async Task<LoginResponseDTO> Login(LoginDTO loginDto)
        {
            if (loginDto.UserType.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                var admin = await _context.Admins
                    .Include(a => a.Member)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Member.Username == loginDto.Username && a.Member.Password == loginDto.Password);

                if (admin != null)
                {
                    var token = GenerateToken(admin.AdminId, admin.Username, "Admin");
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Message = "تم تسجيل الدخول بنجاح",
                        Token = token,
                        UserType = "Admin",
                        UserId = admin.AdminId,
                        FirstName = admin.FirstName,
                        FullName = admin.FirstName + " " + admin.LastName,
                        Gender = admin.Gender
                    };
                }
            }
            else if (loginDto.UserType.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var student = await _context.Students
                    .Include(s => s.Member)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Member.Username == loginDto.Username && s.Member.Password == loginDto.Password);

                if (student != null)
                {
                    var token = GenerateToken(student.StudentId, student.Username, "Student");
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Message = "تم تسجيل الدخول بنجاح",
                        Token = token,
                        UserType = "Student",
                        UserId = student.StudentId,
                        FirstName = student.FirstName,
                        FullName = student.FirstName + " " + student.LastName,
                        Gender = student.Gender
                    };
                }
            }
            else if (loginDto.UserType.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
            {
                var lecturer = await _context.Lecturers
                    .Include(l => l.Member)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Member.Username == loginDto.Username && l.Member.Password == loginDto.Password);

                if (lecturer != null)
                {
                    var token = GenerateToken(lecturer.LecturerId, lecturer.Username, "Lecturer");
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Message = "تم تسجيل الدخول بنجاح",
                        Token = token,
                        UserType = "Lecturer",
                        UserId = lecturer.LecturerId,
                        FirstName = lecturer.FirstName,
                        FullName = lecturer.FirstName + " " + lecturer.LastName,
                        Gender = lecturer.Gender
                    };
                }
            }

            
            return new LoginResponseDTO
            {
                Success = false,
                Message = "اسم المستخدم أو كلمة المرور غير صحيحة",
                Token = "",
                UserType = "",
                UserId = 0
            };
        }

        
        private string GenerateToken(int userId, string username, string userType)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim("Username", username),
                new Claim("UserType", userType),
                new Claim(ClaimTypes.Role, userType)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}