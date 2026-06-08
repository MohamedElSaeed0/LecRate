using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LecRate.Data;
using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly AppDbContext _context;
        private readonly string _hashSecret;

        public EvaluationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _hashSecret = configuration["AnonymousEval:SecretKey"]
                ?? throw new InvalidOperationException("AnonymousEval:SecretKey غير موجود في الإعدادات");
        }

        public async Task<Evaluation> AddEvaluation(int studentId, EvaluationDTO dto)
        {
            if (studentId <= 0 || dto.LecturerId <= 0 || dto.SubjectId <= 0 || dto.QuestionId <= 0)
            {
                throw new InvalidOperationException("بيانات التقييم غير مكتملة (معرفات غير صالحة).");
            }

            if (string.IsNullOrWhiteSpace(dto.TextAnswer))
            {
                throw new InvalidOperationException("الإجابة مطلوبة.");
            }

            var isEnrolled = await _context.StudentSubjects.AnyAsync(ss =>
                ss.StudentId == studentId &&
                ss.SubjectId == dto.SubjectId &&
                ss.LecturerId == dto.LecturerId);

            if (!isEnrolled)
            {
                throw new InvalidOperationException("أنت غير مسجل في هذه المادة مع هذا المحاضر.");
            }

            var hash = AnonymousHashHelper.Generate(_hashSecret, studentId, dto.LecturerId, dto.SubjectId, dto.QuestionId);

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

        public async Task<int> GetStudentEvaluationCount(int studentId)
        {
            var enrollments = await _context.StudentSubjects.AsNoTracking()
                .Where(ss => ss.StudentId == studentId && ss.LecturerId != null)
                .ToListAsync();

            if (!enrollments.Any())
            {
                return 0;
            }

            var questionIds = await _context.Questions.AsNoTracking()
                .Select(q => q.QuestionId)
                .ToListAsync();

            if (!questionIds.Any())
            {
                return 0;
            }

            var activeHashes = (await _context.Evaluations.AsNoTracking()
                .Where(e => !e.IsArchived)
                .Select(e => e.AnonymousHash)
                .ToListAsync()).ToHashSet();

            var count = 0;
            foreach (var enrollment in enrollments)
            {
                foreach (var questionId in questionIds)
                {
                    var hash = AnonymousHashHelper.Generate(
                        _hashSecret,
                        studentId,
                        enrollment.LecturerId!.Value,
                        enrollment.SubjectId,
                        questionId);

                    if (activeHashes.Contains(hash))
                    {
                        count++;
                    }
                }
            }

            return count;
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
