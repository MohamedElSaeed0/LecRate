using ProfRate.DTOs;

namespace ProfRate.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginDTO loginDto);
    }
}