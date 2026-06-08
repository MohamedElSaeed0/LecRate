using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LecRate.DTOs;
using LecRate.Services;

namespace LecRate.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        
        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var result = await _studentService.GetAllStudents(page, pageSize);
            
            
            var safeResult = new
            {
                items = result.Items.Select(s => new StudentResponseDTO
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Username = s.Username,
                    AdminId = s.AdminId
                }),
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                result.TotalPages,
                result.HasPrevious,
                result.HasNext
            };
            return Ok(safeResult);
        }

        
        [HttpGet]
        [Route("Search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var students = await _studentService.Search(query);
            var safeStudents = students.Select(s => new StudentResponseDTO
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Username = s.Username,
                AdminId = s.AdminId
            });
            return Ok(safeStudents);
        }

        
        
        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentById(id);
            if (student == null)
            {
                return NotFound(new { message = "الطالب غير موجود" });
            }
            
            return Ok(student);
        }

        
        
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AddStudent([FromBody] StudentDTO dto)
        {
            try
            {
                var student = await _studentService.AddStudent(dto);
                return Ok(new { message = "تمت إضافة الطالب بنجاح", student });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء إضافة الطالب" });
            }
        }

        
        
        [HttpPut]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDTO dto)
        {
            var student = await _studentService.UpdateStudent(id, dto);
            if (student == null)
            {
                return NotFound(new { message = "الطالب غير موجود" });
            }
            return Ok(new { message = "تم تعديل الطالب بنجاح", student });
        }

        
        
        [HttpDelete]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);
            if (!result)
            {
                return NotFound(new { message = "الطالب غير موجود" });
            }
            return Ok(new { message = "تم حذف الطالب بنجاح" });
        }
    }
}