using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly PasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public AuthController(AuthenticationService authenticationService, PasswordHasher passwordHasher, IUserRepository userRepository)
    {
        _authenticationService = authenticationService;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid registration details");
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Usuario = request.Username,
            Senha = hashedPassword,
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        await _userRepository.AddAsync(user);

        return Ok("User registered successfully");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid username or password");
        }

        var user = await _authenticationService.AuthenticateAsync(request.Username, request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        var token = _authenticationService.GenerateJwtToken(user);
        var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(user);

        return Ok(new { Token = token, RefreshToken = refreshToken });
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Invalid refresh token");
        }

        var user = await _authenticationService.ValidateRefreshTokenAsync(request.RefreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        var token = _authenticationService.GenerateJwtToken(user);
        var newRefreshToken = await _authenticationService.GenerateRefreshTokenAsync(user);

        return Ok(new { Token = token, RefreshToken = newRefreshToken });
    }
}

