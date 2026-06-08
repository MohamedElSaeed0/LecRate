using ProfRate.DTOs;
using ProfRate.Entities;

namespace ProfRate.Services
{
    public interface ISubjectService
    {
        Task<Subject?> AddSubject(SubjectDTO dto);
        Task<bool> DeleteSubject(int id);
        Task<List<Subject>> GetAllSubjects();
        Task<Subject?> GetSubjectById(int id);
    }
}