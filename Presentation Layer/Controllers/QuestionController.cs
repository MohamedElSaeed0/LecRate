using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.DTOs;
using LecRate.Services;

namespace LecRate.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        
        
        [HttpGet]
        [Route("GetAll")]
        [Authorize] 
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestions();
            return Ok(questions);
        }

        
        
        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize] 
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _questionService.GetQuestionById(id);
            if (question == null)
            {
                return NotFound(new { message = "السؤال غير موجود" });
            }
            return Ok(question);
        }

        
        
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AddQuestion([FromBody] QuestionDTO dto)
        {
            var question = await _questionService.AddQuestion(dto);
            return Ok(new { message = "تمت إضافة السؤال بنجاح", question });
        }

        
        
        [HttpPut]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionDTO dto)
        {
            var question = await _questionService.UpdateQuestion(id, dto);
            if (question == null)
            {
                return NotFound(new { message = "السؤال غير موجود" });
            }
            return Ok(new { message = "تم تعديل السؤال بنجاح", question });
        }

        
        
        [HttpDelete]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await _questionService.DeleteQuestion(id);
            if (!result)
            {
                return NotFound(new { message = "السؤال غير موجود" });
            }
            return Ok(new { message = "تم حذف السؤال بنجاح" });
        }
    }
}
