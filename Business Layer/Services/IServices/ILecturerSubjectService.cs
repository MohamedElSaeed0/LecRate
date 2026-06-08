using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface ILecturerSubjectService
    {
        Task<(bool Success, string Message)> AddLecturerSubject(LecturerSubjectDTO dto);
        Task<bool> DeleteLecturerSubject(int id);
        Task<List<LecturerSubject>> GetAll();
        Task<List<LecturerSubject>> GetByLecturer(int lecturerId);
        Task<List<LecturerSubject>> GetBySubject(int subjectId);
        Task<(bool Success, string Message)> UpdateLecturerSubject(int id, LecturerSubjectDTO dto);
    }
}