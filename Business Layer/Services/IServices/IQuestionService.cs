using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface IQuestionService
    {
        Task<Question> AddQuestion(QuestionDTO dto);
        Task<bool> DeleteQuestion(int id);
        Task<List<Question>> GetAllQuestions();
        Task<Question?> GetQuestionById(int id);
        Task<Question?> UpdateQuestion(int id, QuestionDTO dto);
    }
}