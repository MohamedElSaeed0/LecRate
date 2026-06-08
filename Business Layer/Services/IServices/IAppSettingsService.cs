
namespace ProfRate.Services
{
    public interface IAppSettingsService
    {
        Task<bool> IsEvaluationOpen();
        Task<bool> ToggleEvaluation(bool isOpen);
    }
}