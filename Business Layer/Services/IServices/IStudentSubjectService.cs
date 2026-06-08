using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface IStudentSubjectService
    {
        Task<(bool Success, string Message)> AddStudentSubject(StudentSubjectDTO dto);
        Task<bool> DeleteStudentSubject(int id);
        Task<List<StudentSubject>> GetAll();
        Task<List<StudentSubject>> GetByStudent(int studentId);
        Task<(bool Success, string Message)> UpdateStudentSubject(int id, StudentSubjectDTO dto);
    }
}