using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface IEvaluationService
    {
        Task<Evaluation> AddEvaluation(EvaluationDTO dto);
        Task<List<EvaluationResponseDTO>> GetAllEvaluations();
        Task<List<EvaluationReportDTO>> GetEvaluationReport();
        Task<List<EvaluationResponseDTO>> GetEvaluationsByLecturer(int lecturerId);
        Task<bool> ResetEvaluations();
    }
}