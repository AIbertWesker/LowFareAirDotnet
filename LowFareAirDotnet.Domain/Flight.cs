namespace LowFareAirDotnet.Domain;

public class Flight
{
    public string FlightId { get; set; } = null!;
    public int SegmentNumber { get; set; }
    public string? OrigAirport { get; set; }
    public TimeOnly? DepartTime { get; set; }
    public string? DestAirport { get; set; }
    public TimeOnly? ArriveTime { get; set; }
    public string? Meal { get; set; }
    public double? FlyingTime { get; set; }
    public int? Miles { get; set; }
    public string? Aircraft { get; set; }
}
