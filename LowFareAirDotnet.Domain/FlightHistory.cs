namespace LowFareAirDotnet.Domain;

public class FlightHistory
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FlightId { get; set; } = null!;
    public string OrigAirport { get; set; } = null!;
    public string DestAirport { get; set; } = null!;
    public string? BeginDate { get; set; }
    public string? Class { get; set; }
}
