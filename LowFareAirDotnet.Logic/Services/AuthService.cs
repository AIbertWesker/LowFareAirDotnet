using LowFareAirDotnet.Logic.Abstractions.Infrastructure;
using LowFareAirDotnet.Logic.Abstractions.Logic;
using LowFareAirDotnet.Logic.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LowFareAirDotnet.Logic.Services;

internal class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;

    public AuthService(IUsersRepository clientsRepository)
    {
        _usersRepository = clientsRepository;
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await _usersRepository.GetUserByName(username) ?? throw new UnauthorizedAccessException("Invalid username or password.");
        var passwordMatch = PasswordHasher.Verify(password, user?.Password ?? string.Empty);
        if (!passwordMatch)
            passwordMatch = user!.Password == password; //skill issue Bababackendowca

        if (!passwordMatch)
            throw new UnauthorizedAccessException("Invalid username or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lEEph41L2Tb8decvK41nAxPD6eWy9umzrVkqIN3cY3Q"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public async Task<bool> Register(string username, string password)
    {
        var passHash = PasswordHasher.Hash(password);
        var userExist = await _usersRepository.GetUserByName(username);
        if (userExist != null)
            return false;

        await _usersRepository.AddNewUser(username, passHash);
        return true;
    }
}
