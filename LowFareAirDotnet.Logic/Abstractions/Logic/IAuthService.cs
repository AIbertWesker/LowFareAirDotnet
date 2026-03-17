namespace LowFareAirDotnet.Logic.Abstractions.Logic;

public interface IAuthService
{
    public Task<string> Login(string username, string password);
    Task<bool> Register(string username, string password);
}
