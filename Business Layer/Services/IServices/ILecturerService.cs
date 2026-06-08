using LecRate.DTOs;
using LecRate.Entities;

namespace LecRate.Services
{
    public interface ILecturerService
    {
        Task<Lecturer> AddLecturer(LecturerDTO dto);
        Task<bool> DeleteLecturer(int id);
        Task<List<Lecturer>> GetAllLecturers();
        Task<Lecturer?> GetLecturerById(int id);
        Task<List<Lecturer>> Search(string query);
        Task<Lecturer?> UpdateLecturer(int id, LecturerDTO dto);
        Task<Lecturer?> UpdateAdminRating(int id, int rating); 
    }
}