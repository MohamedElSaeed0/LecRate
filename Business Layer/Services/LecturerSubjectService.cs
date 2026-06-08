using Microsoft.EntityFrameworkCore;
using ProfRate.Data;
using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    
    public class LecturerSubjectService : ILecturerSubjectService
    {
        private readonly AppDbContext _context;

        public LecturerSubjectService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<LecturerSubject>> GetAll()
        {
            return await _context.LecturerSubjects
                .Include(ls => ls.Lecturer)
                    .ThenInclude(l => l.Member)
                .Include(ls => ls.Subject)
                .OrderBy(ls => ls.Lecturer.Member.FirstName)
                .ToListAsync();
        }

        
        public async Task<List<LecturerSubject>> GetByLecturer(int lecturerId)
        {
            return await _context.LecturerSubjects
                .Where(ls => ls.LecturerId == lecturerId)
                .Include(ls => ls.Subject)
                .ToListAsync();
        }

        
        public async Task<List<LecturerSubject>> GetBySubject(int subjectId)
        {
            return await _context.LecturerSubjects
                .Where(ls => ls.SubjectId == subjectId)
                .Include(ls => ls.Lecturer)
                    .ThenInclude(l => l.Member)
                .OrderBy(ls => ls.Lecturer.Member.FirstName)
                .ToListAsync();
        }

        
        public async Task<(bool Success, string Message)> AddLecturerSubject(LecturerSubjectDTO dto)
        {
            var exists = await _context.LecturerSubjects
                .AnyAsync(ls => ls.LecturerId == dto.LecturerId && ls.SubjectId == dto.SubjectId);

            if (exists)
            {
                return (false, "هذا المحاضر معين بالفعل لهذه المادة");
            }

            var entry = new LecturerSubject
            {
                LecturerId = dto.LecturerId,
                SubjectId = dto.SubjectId
            };

            _context.LecturerSubjects.Add(entry);
            await _context.SaveChangesAsync();
            return (true, "تم ربط المحاضر بالمادة بنجاح");
        }

        
        public async Task<(bool Success, string Message)> UpdateLecturerSubject(int id, LecturerSubjectDTO dto)
        {
            var entry = await _context.LecturerSubjects.FindAsync(id);
            if (entry == null) return (false, "هذا السجل غير موجود");

            bool exists = await _context.LecturerSubjects
                .AnyAsync(ls => ls.LecturerId == dto.LecturerId && ls.SubjectId == dto.SubjectId && ls.LecturerSubjectId != id);

            if (exists)
            {
                return (false, "هذا المحاضر معين بالفعل لهذه المادة");
            }

            
            var oldLecturerId = entry.LecturerId;
            var oldSubjectId = entry.SubjectId;

            
            entry.LecturerId = dto.LecturerId;
            entry.SubjectId = dto.SubjectId;

            
            var affectedStudents = await _context.StudentSubjects
                .Where(ss => ss.LecturerId == oldLecturerId && ss.SubjectId == oldSubjectId)
                .ToListAsync();

            if (affectedStudents.Any())
            {
                foreach (var studentSubject in affectedStudents)
                {
                    studentSubject.LecturerId = dto.LecturerId;
                    studentSubject.SubjectId = dto.SubjectId; 
                }
            }

            await _context.SaveChangesAsync();
            return (true, "تم تعديل الربط بنجاح (وتم تحديث بيانات الطلاب المرتبطين)");
        }

        
        public async Task<bool> DeleteLecturerSubject(int id)
        {
            var entry = await _context.LecturerSubjects.FindAsync(id);
            if (entry == null) return false;

            _context.LecturerSubjects.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}