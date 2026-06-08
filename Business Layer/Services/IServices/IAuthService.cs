using LecRate.DTOs;

namespace LecRate.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginDTO loginDto);
    }
}