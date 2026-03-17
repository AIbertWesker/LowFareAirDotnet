using LowFareAirDotnet.Logic.Abstractions.Logic;
using LowFareAirDotnet.Web.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LowFareAirDotnet.Web.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.Login(request.Username, request.Password);
        if (token is null)
            return Unauthorized();

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.Register(request.Username, request.Password);
        
        if (!result)
            return BadRequest("User already exists");

        return Ok(result);
    }
}