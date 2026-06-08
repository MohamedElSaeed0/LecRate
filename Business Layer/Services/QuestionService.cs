using Microsoft.EntityFrameworkCore;
using LecRate.Data;
using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    
    public class QuestionService : IQuestionService
    {
        private readonly AppDbContext _context;

        public QuestionService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<Question>> GetAllQuestions()
        {
            return await _context.Questions.ToListAsync();
        }

        
        public async Task<Question?> GetQuestionById(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        
        public async Task<Question> AddQuestion(QuestionDTO dto)
        {
            var question = new Question
            {
                QuestionText = dto.QuestionText,
                AdminId = dto.AdminId
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        
        public async Task<Question?> UpdateQuestion(int id, QuestionDTO dto)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return null;

            question.QuestionText = dto.QuestionText;

            await _context.SaveChangesAsync();
            return question;
        }

        
        
        public async Task<bool> DeleteQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Evaluations)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null) return false;

            if (question.Evaluations.Any())
                _context.Evaluations.RemoveRange(question.Evaluations);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}