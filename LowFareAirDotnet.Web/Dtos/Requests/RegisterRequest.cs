namespace LowFareAirDotnet.Web.Dtos.Requests;

public sealed class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
