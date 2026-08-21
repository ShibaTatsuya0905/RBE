using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IUserRepository userRepository, IConfiguration configuration) { _userRepository = userRepository; _configuration = configuration; }
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) throw new Exception("Invalid credentials");
        return new AuthResponse { Token = GenerateJwtToken(user), UserId = user.Id, Username = user.Username, FullName = user.FullName, Role = user.Role.ToString() };
    }
    public async Task RegisterTestAdminAsync()
    {
        if (await _userRepository.GetByUsernameAsync("admin") == null)
            await _userRepository.CreateAsync(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), FullName = "Admin", Role = UserRole.Admin });
    }
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role.ToString()) };
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}