using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.DTOs;
using LecRate.Services;
using System.Security.Claims;

namespace LecRate.Controllers
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

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst("UserId")?.Value;
            return int.TryParse(claim, out userId);
        }

        private string? GetUserType() => User.FindFirst("UserType")?.Value;

        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEvaluations()
        {
            var evaluations = await _evaluationService.GetAllEvaluations();
            return Ok(evaluations);
        }

        [HttpGet]
        [Route("GetByLecturer/{lecturerId}")]
        [Authorize]
        public async Task<IActionResult> GetEvaluationsByLecturer(int lecturerId)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized(new { message = "جلسة غير صالحة" });
            }

            var userType = GetUserType();
            if (userType == "Lecturer" && userId != lecturerId)
            {
                return Forbid();
            }

            if (userType != "Admin" && userType != "Lecturer")
            {
                return Forbid();
            }

            var evaluations = await _evaluationService.GetEvaluationsByLecturer(lecturerId);
            return Ok(evaluations);
        }

        [HttpGet]
        [Route("GetReport")]
        [Authorize]
        public async Task<IActionResult> GetEvaluationReport()
        {
            var report = await _evaluationService.GetEvaluationReport();
            return Ok(report);
        }

        [HttpGet]
        [Route("GetMyCount")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEvaluationCount()
        {
            if (!TryGetUserId(out int studentId))
            {
                return Unauthorized(new { message = "جلسة غير صالحة" });
            }

            var count = await _evaluationService.GetStudentEvaluationCount(studentId);
            return Ok(new { count });
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

            if (!TryGetUserId(out int studentId))
            {
                return Unauthorized(new { message = "جلسة غير صالحة" });
            }

            try
            {
                var evaluation = await _evaluationService.AddEvaluation(studentId, dto);
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
