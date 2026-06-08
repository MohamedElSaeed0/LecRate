using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfRate.DTOs;
using ProfRate.Services;

namespace ProfRate.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        
        
        [HttpGet]
        [Route("GetAll")]
        [Authorize] 
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
            return Ok(subjects);
        }

        
        
        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize] 
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectById(id);
            if (subject == null)
            {
                return NotFound(new { message = "المادة غير موجودة" });
            }
            return Ok(subject);
        }

        
        
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AddSubject([FromBody] SubjectDTO dto)
        {
            var subject = await _subjectService.AddSubject(dto);
            if (subject == null)
            {
                return BadRequest(new { message = "المادة موجودة مسبقاً" });
            }
            return Ok(new { message = "تمت إضافة المادة بنجاح", subject });
        }



        
        
        [HttpDelete]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteSubject(id);
            if (!result)
            {
                return NotFound(new { message = "المادة غير موجودة" });
            }
            return Ok(new { message = "تم حذف المادة بنجاح" });
        }
    }
}
