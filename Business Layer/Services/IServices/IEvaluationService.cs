using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    public interface IEvaluationService
    {
        Task<Evaluation> AddEvaluation(int studentId, EvaluationDTO dto);
        Task<int> GetStudentEvaluationCount(int studentId);
        Task<List<EvaluationResponseDTO>> GetAllEvaluations();
        Task<List<EvaluationReportDTO>> GetEvaluationReport();
        Task<List<EvaluationResponseDTO>> GetEvaluationsByLecturer(int lecturerId);
        Task<bool> ResetEvaluations();
    }
}
