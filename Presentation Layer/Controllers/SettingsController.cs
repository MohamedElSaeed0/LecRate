using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.Services;

namespace LecRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IAppSettingsService _settingsService;

        public SettingsController(IAppSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        
        
        [HttpGet]
        [Route("IsEvaluationOpen")]
        [AllowAnonymous] 
        public async Task<IActionResult> IsEvaluationOpen()
        {
            var isOpen = await _settingsService.IsEvaluationOpen();
            return Ok(new { isOpen });
        }

        
        
        [HttpPost]
        [Route("OpenEvaluation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OpenEvaluation()
        {
            await _settingsService.ToggleEvaluation(true);
            return Ok(new { message = "تم فتح التقييم للطلاب", isOpen = true });
        }

        
        
        [HttpPost]
        [Route("CloseEvaluation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CloseEvaluation()
        {
            await _settingsService.ToggleEvaluation(false);
            return Ok(new { message = "تم قفل التقييم", isOpen = false });
        }
    }
}