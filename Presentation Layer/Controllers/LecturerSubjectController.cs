using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.DTOs;
using LecRate.Services;
using System.Security.Claims;

namespace LecRate.Controllers
{
    [Route("api/lecturersubjects")]
    [ApiController]
    public class LecturerSubjectController : ControllerBase
    {
        private readonly ILecturerSubjectService _service;

        public LecturerSubjectController(ILecturerSubjectService service)
        {
            _service = service;
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
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAll();
            return Ok(list);
        }

        [HttpGet]
        [Route("GetByLecturer/{lecturerId}")]
        [Authorize]
        public async Task<IActionResult> GetByLecturer(int lecturerId)
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

            var list = await _service.GetByLecturer(lecturerId);
            return Ok(list);
        }

        [HttpGet]
        [Route("GetBySubject/{subjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBySubject(int subjectId)
        {
            var list = await _service.GetBySubject(subjectId);
            return Ok(list);
        }

        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] LecturerSubjectDTO model)
        {
            var result = await _service.AddLecturerSubject(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPut]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] LecturerSubjectDTO model)
        {
            var result = await _service.UpdateLecturerSubject(id, model);
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
            var success = await _service.DeleteLecturerSubject(id);
            if (!success)
            {
                return NotFound(new { message = "غير موجود" });
            }
            return Ok(new { message = "تم الحذف بنجاح" });
        }
    }
}
