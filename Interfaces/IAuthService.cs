using RestaurantManagement.API.DTOs;
namespace RestaurantManagement.API.Interfaces;
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task RegisterTestAdminAsync();
}