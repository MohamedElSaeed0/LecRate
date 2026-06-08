using Microsoft.EntityFrameworkCore;
using ProfRate.Data;
using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<PagedResult<Student>> GetAllStudents(int page = 1, int pageSize = 20)
        {
            var totalStudents = await _context.Students.CountAsync();

            var students = await _context.Students
                .Include(s => s.Member)
                .AsNoTracking()
                .OrderBy(s => s.Member.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Student>
            {
                Items = students,
                TotalCount = totalStudents,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalStudents / (double)pageSize)
            };
        }

        
        public async Task<List<Student>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Student>();

            
            query = query.Trim();
            if (query.Length > 100) query = query.Substring(0, 100);
            query = System.Text.RegularExpressions.Regex.Replace(query, @"[^\w\s\u0600-\u06FF]", "");

            return await _context.Students
                .Include(s => s.Member)
                .AsNoTracking()
                .Where(s => s.Member.Username.Contains(query) ||
                            s.Member.FirstName.Contains(query) ||
                            s.Member.LastName.Contains(query))
                .OrderBy(s => s.Member.FirstName)
                .Take(100)
                .ToListAsync();
        }

        
        public async Task<Student?> GetStudentById(int id)
        {
            return await _context.Students
                .Include(s => s.Member)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        
        public async Task<Student> AddStudent(StudentDTO dto)
        {
            
            if (await _context.Members.AnyAsync(m => m.Username == dto.Username))
                throw new InvalidOperationException("اسم المستخدم موجود بالفعل");

            var student = new Student
            {
                AdminId = dto.AdminId,
                Member = new Member
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    Password = dto.Password, 
                    Gender = (byte)(dto.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                }
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        
        public async Task<Student?> UpdateStudent(int id, StudentDTO dto)
        {
            var student = await _context.Students
                .Include(s => s.Member)
                .FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null) return null;

            student.Member.FirstName = dto.FirstName;
            student.Member.LastName = dto.LastName;
            student.Member.Username = dto.Username;
            student.Member.Password = dto.Password; 
            student.Member.Gender = (byte)(dto.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? 1 : 0);

            await _context.SaveChangesAsync();
            return student;
        }

        
        public async Task<bool> DeleteStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Member)
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return false;

            if (student.StudentSubjects.Any())
                _context.StudentSubjects.RemoveRange(student.StudentSubjects);

            if (student.Member != null)
            {
                _context.Members.Remove(student.Member);
            }
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}