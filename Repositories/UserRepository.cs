using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Repositories;
public class UserRepository : IUserRepository
{
    private readonly RestaurantDbContext _context;
    public UserRepository(RestaurantDbContext context) { _context = context; }
    public async Task<User?> GetByUsernameAsync(string username) => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    public async Task<User> CreateAsync(User user) { _context.Users.Add(user); await _context.SaveChangesAsync(); return user; }
}