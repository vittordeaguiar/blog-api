using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.API.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        await authService.RegisterAsync(dto);
        return Created(string.Empty, new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await authService.LoginAsync(dto);
        return Ok(response);
    }
}