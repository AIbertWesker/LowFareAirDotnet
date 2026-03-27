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

        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(12)
        });

        return Ok(token);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        return Ok();
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