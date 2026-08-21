using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) { _authService = authService; }
    [HttpPost("login")] public async Task<IActionResult> Login([FromBody] LoginRequest request) { try { return Ok(await _authService.LoginAsync(request)); } catch (Exception ex) { return Unauthorized(new { message = ex.Message }); } }
    [HttpPost("setup-admin")] public async Task<IActionResult> SetupAdmin() { await _authService.RegisterTestAdminAsync(); return Ok(); }
}