using Microsoft.EntityFrameworkCore;
using ProfRate.Data;
using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    
    public class EvaluationService : IEvaluationService
    {
        private readonly AppDbContext _context;

        public EvaluationService(AppDbContext context)
        {
            _context = context;
        }

        private const string HASH_SECRET = "EvalProf_AnonymousEval_2026_SecretKey";

        private string GenerateAnonymousHash(int studentId, int lecturerId, int subjectId, int questionId)
        {
            var raw = $"{studentId}|{lecturerId}|{subjectId}|{questionId}|{HASH_SECRET}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }

        
        public async Task<Evaluation> AddEvaluation(EvaluationDTO dto)
        {
            if (dto.StudentId <= 0 || dto.LecturerId <= 0 || dto.SubjectId <= 0 || dto.QuestionId <= 0)
            {
                throw new InvalidOperationException("بيانات التقييم غير مكتملة (معرفات غير صالحة).");
            }

            if (string.IsNullOrWhiteSpace(dto.TextAnswer))
            {
                throw new InvalidOperationException("الإجابة مطلوبة.");
            }

            
            var hash = GenerateAnonymousHash(dto.StudentId, dto.LecturerId, dto.SubjectId, dto.QuestionId);

            
            var exists = await _context.Evaluations
                .AnyAsync(e => e.AnonymousHash == hash && !e.IsArchived);

            if (exists)
            {
                throw new InvalidOperationException("لقد قمت بتقييم هذا السؤال للمحاضر مسبقاً.");
            }

            var evaluation = new Evaluation
            {
                TextAnswer = dto.TextAnswer,
                AnonymousHash = hash,
                QuestionId = dto.QuestionId,
                LecturerId = dto.LecturerId,
                SubjectId = dto.SubjectId,
                IsArchived = false
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();
            return evaluation;
        }

        
        
        public async Task<bool> ResetEvaluations()
        {
            var activeEvaluations = await _context.Evaluations
                .Where(e => !e.IsArchived)
                .ToListAsync();

            if (activeEvaluations.Any())
            {
                
                foreach (var eval in activeEvaluations)
                {
                    eval.IsArchived = true;
                }

                
                var lecturers = await _context.Lecturers.ToListAsync();
                foreach (var lecturer in lecturers)
                {
                    lecturer.AdminRating = null;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        
        public async Task<List<EvaluationResponseDTO>> GetEvaluationsByLecturer(int lecturerId)
        {
            return await _context.Evaluations.AsNoTracking()
                .Include(e => e.Question)
                .Include(e => e.Subject)
                .Where(e => e.LecturerId == lecturerId && !e.IsArchived)
                .Select(e => new EvaluationResponseDTO
                {
                    EvaluationId = e.EvaluationId,
                    TextAnswer = e.TextAnswer,
                    IsArchived = e.IsArchived,
                    LecturerId = e.LecturerId,
                    StudentName = "طالب", 
                    LecturerName = "",
                    SubjectName = e.Subject.SubjectName,
                    QuestionText = e.Question.QuestionText
                })
                .ToListAsync();
        }

        
        public async Task<List<EvaluationReportDTO>> GetEvaluationReport()
        {
            var lecturers = await _context.Lecturers.AsNoTracking()
                .Select(l => new EvaluationReportDTO
                {
                    LecturerId = l.LecturerId,
                    LecturerName = l.Member.FirstName + " " + l.Member.LastName,
                    SubjectName = "", 
                    AverageRating = l.AdminRating ?? 0, 
                    TotalEvaluations = _context.Evaluations.Count(e => e.LecturerId == l.LecturerId && !e.IsArchived)
                })
                .ToListAsync();

            return lecturers;
        }

        
        public async Task<List<EvaluationResponseDTO>> GetAllEvaluations()
        {
            return await _context.Evaluations.AsNoTracking()
                .Include(e => e.Lecturer)
                .ThenInclude(l => l.Member)
                .Include(e => e.Subject)
                .Include(e => e.Question)
                .Where(e => !e.IsArchived)
                .Select(e => new EvaluationResponseDTO
                {
                    EvaluationId = e.EvaluationId,
                    TextAnswer = e.TextAnswer,
                    IsArchived = e.IsArchived,
                    LecturerId = e.LecturerId,
                    StudentName = "مشارك مجهول",
                    LecturerName = e.Lecturer.Member.FirstName + " " + e.Lecturer.Member.LastName,
                    SubjectName = e.Subject.SubjectName,
                    QuestionText = e.Question.QuestionText
                })
                .ToListAsync();
        }
    }
}