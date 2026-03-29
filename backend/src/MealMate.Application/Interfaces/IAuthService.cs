using MealMate.Application.Common;
using MealMate.Application.DTOs.Auth;

namespace MealMate.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ServiceResult<bool>> RevokeTokenAsync(Guid userId);
}
