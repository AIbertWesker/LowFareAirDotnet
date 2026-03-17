namespace LowFareAirDotnet.Web.Dtos.Requests;

public sealed class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
