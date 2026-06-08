using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.Services;
using System.Security.Claims;

namespace LecRate.Controllers
{
    [Route("api/studentsubjects")]
    [ApiController]
    public class StudentSubjectController : ControllerBase
    {
        private readonly IStudentSubjectService _service;

        public StudentSubjectController(IStudentSubjectService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAll();
            return Ok(list);
        }

        [HttpGet]
        [Route("GetByStudent/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var userType = User.FindFirst("UserType")?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userType == "Student")
            {
                if (!int.TryParse(userIdClaim, out int currentStudentId) || currentStudentId != studentId)
                {
                    return Forbid();
                }
            }
            else if (userType != "Admin")
            {
                return Forbid();
            }

            var list = await _service.GetByStudent(studentId);
            return Ok(list);
        }

        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] LecRate.DTOs.StudentSubjectDTO model)
        {
            var result = await _service.AddStudentSubject(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPut]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] LecRate.DTOs.StudentSubjectDTO model)
        {
            var result = await _service.UpdateStudentSubject(id, model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteStudentSubject(id);
            if (!success)
            {
                return NotFound(new { message = "غير موجود" });
            }
            return Ok(new { message = "تم الحذف بنجاح" });
        }
    }
}
