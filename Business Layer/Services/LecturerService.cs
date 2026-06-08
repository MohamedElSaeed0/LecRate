using Microsoft.EntityFrameworkCore;
using LecRate.Data;
using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    
    public class LecturerService : ILecturerService
    {
        private readonly AppDbContext _context;

        public LecturerService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<Lecturer>> GetAllLecturers()
        {
            return await _context.Lecturers
                .Include(l => l.Member)
                .Include(l => l.LecturerSubjects)
                .ThenInclude(ls => ls.Subject)
                .AsNoTracking()
                .OrderBy(l => l.Member.FirstName)
                .ToListAsync();
        }

        
        public async Task<List<Lecturer>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Lecturer>();

            
            query = query.Trim();
            if (query.Length > 100) query = query.Substring(0, 100);
            query = System.Text.RegularExpressions.Regex.Replace(query, @"[^\w\s\u0600-\u06FF]", "");

            return await _context.Lecturers
                .Include(l => l.Member)
                .Include(l => l.LecturerSubjects)
                .ThenInclude(ls => ls.Subject)
                .AsNoTracking()
                .Where(l => l.Member.Username.Contains(query) ||
                            l.Member.FirstName.Contains(query) ||
                            l.Member.LastName.Contains(query))
                .OrderBy(l => l.Member.FirstName)
                .Take(100)
                .ToListAsync();
        }

        
        public async Task<Lecturer?> GetLecturerById(int id)
        {
            return await _context.Lecturers
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.LecturerId == id);
        }

        
        public async Task<Lecturer> AddLecturer(LecturerDTO dto)
        {
            if (await _context.Members.AnyAsync(m => m.Username == dto.Username))
                throw new InvalidOperationException("اسم المستخدم موجود بالفعل");

            var lecturer = new Lecturer
            {
                AdminId = dto.AdminId,
                AdminRating = (byte?)dto.AdminRating,
                Member = new Member
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    Password = dto.Password, 
                    Gender = (byte)(dto.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                }
            };

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();
            return lecturer;
        }

        
        public async Task<Lecturer?> UpdateLecturer(int id, LecturerDTO dto)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.LecturerId == id);
            if (lecturer == null) return null;

            if (await _context.Members.AnyAsync(m => m.Username == dto.Username && m.MemberId != lecturer.MemberId))
                throw new InvalidOperationException("اسم المستخدم موجود بالفعل");

            lecturer.Member.FirstName = dto.FirstName;
            lecturer.Member.LastName = dto.LastName;
            lecturer.Member.Username = dto.Username;
            lecturer.Member.Password = dto.Password; 
            lecturer.Member.Gender = (byte)(dto.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? 1 : 0);

            await _context.SaveChangesAsync();
            return lecturer;
        }

        
        public async Task<bool> DeleteLecturer(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.Member)
                .Include(l => l.Evaluations)
                .Include(l => l.LecturerSubjects)
                .FirstOrDefaultAsync(l => l.LecturerId == id);

            if (lecturer == null) return false;

            
            if (lecturer.Evaluations.Any())
                _context.Evaluations.RemoveRange(lecturer.Evaluations);

            if (lecturer.LecturerSubjects.Any())
                _context.LecturerSubjects.RemoveRange(lecturer.LecturerSubjects);

            
            var studentSubjects = await _context.StudentSubjects.Where(ss => ss.LecturerId == id).ToListAsync();
            if (studentSubjects.Any())
                _context.StudentSubjects.RemoveRange(studentSubjects);

            if (lecturer.Member != null)
            {
                _context.Members.Remove(lecturer.Member);
            }
            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<Lecturer?> UpdateAdminRating(int id, int rating)
        {
            if (rating < 0 || rating > 100)
                throw new InvalidOperationException("التقييم يجب أن يكون بين 0 و 100");

            var lecturer = await _context.Lecturers
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.LecturerId == id);
            if (lecturer == null) return null;

            
            lecturer.AdminRating = rating == 0 ? null : (byte?)rating;
            await _context.SaveChangesAsync();
            return lecturer;
        }
    }
}