using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly RestaurantDbContext _context;

    public UsersController(RestaurantDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        var result = users.Select(u => new
        {
            u.Id,
            name = u.FullName,
            username = u.Username,
            role = u.Role.ToString(),
            shift = u.PhoneNumber ?? "Morning",
            status = "Active"
        });
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted()
    {
        var deletedUsers = await _context.Users.IgnoreQueryFilters().Where(u => u.IsDeleted).ToListAsync();
        var result = deletedUsers.Select(u => new
        {
            u.Id,
            name = u.FullName,
            username = u.Username,
            role = u.Role.ToString(),
            shift = u.PhoneNumber ?? "Morning",
            status = "Deleted"
        });
        return Ok(result);
    }

    [HttpPut("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        user.IsDeleted = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    public class CreateUserDto
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Shift { get; set; } = null!;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return BadRequest(new { message = "Username already exists" });

        var user = new User
        {
            FullName = request.Name,
            Username = request.Username,
            Role = Enum.Parse<UserRole>(request.Role),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PhoneNumber = request.Shift
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.FullName = request.Name;
        user.PhoneNumber = request.Shift;
        if (user.Username != "admin")
        {
            user.Username = request.Username;
            user.Role = Enum.Parse<UserRole>(request.Role);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }
}