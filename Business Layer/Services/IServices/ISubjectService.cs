using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    public interface ISubjectService
    {
        Task<Subject?> AddSubject(SubjectDTO dto);
        Task<bool> DeleteSubject(int id);
        Task<List<Subject>> GetAllSubjects();
        Task<Subject?> GetSubjectById(int id);
    }
}