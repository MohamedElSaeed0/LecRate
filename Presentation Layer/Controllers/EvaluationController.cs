using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfRate.DTOs;
using ProfRate.Services;

namespace ProfRate.Controllers
{
    [Route("api/evaluations")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        private readonly IAppSettingsService _settingsService;

        public EvaluationController(IEvaluationService evaluationService, IAppSettingsService settingsService)
        {
            _evaluationService = evaluationService;
            _settingsService = settingsService;
        }

        
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllEvaluations()
        {
            var evaluations = await _evaluationService.GetAllEvaluations();
            return Ok(evaluations);
        }

        
        
        [HttpGet]
        [Route("GetByLecturer/{lecturerId}")]
        public async Task<IActionResult> GetEvaluationsByLecturer(int lecturerId)
        {
            var evaluations = await _evaluationService.GetEvaluationsByLecturer(lecturerId);
            return Ok(evaluations);
        }

        
        
        [HttpGet]
        [Route("GetReport")]
        public async Task<IActionResult> GetEvaluationReport()
        {
            var report = await _evaluationService.GetEvaluationReport();
            return Ok(report);
        }

        
        
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Student")] 
        public async Task<IActionResult> AddEvaluation([FromBody] EvaluationDTO dto)
        {
            
            var isOpen = await _settingsService.IsEvaluationOpen();
            if (!isOpen)
            {
                return BadRequest(new { message = "عفواً، التقييم مغلق حالياً. يرجى المحاولة لاحقاً." });
            }

            try
            {
                var evaluation = await _evaluationService.AddEvaluation(dto);
                return Ok(new { message = "تمت إضافة التقييم بنجاح", evaluation });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        
        [HttpPost]
        [Route("Reset")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ResetEvaluations()
        {
            var result = await _evaluationService.ResetEvaluations();
            return Ok(new { message = result ? "تمت أرشفة التقييمات السابقة وبدء دورة جديدة بنجاح" : "لا توجد تقييمات نشطة للأرشفة" });
        }
    }
}