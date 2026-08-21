using RestaurantManagement.API.Entities;
namespace RestaurantManagement.API.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
}