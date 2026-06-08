using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface IStudentService
    {
        Task<Student> AddStudent(StudentDTO dto);
        Task<bool> DeleteStudent(int id);
        Task<PagedResult<Student>> GetAllStudents(int page = 1, int pageSize = 20);
        Task<Student?> GetStudentById(int id);
        Task<List<Student>> Search(string query);
        Task<Student?> UpdateStudent(int id, StudentDTO dto);
    }
}