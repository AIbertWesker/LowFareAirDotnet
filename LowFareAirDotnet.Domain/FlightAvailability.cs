namespace LowFareAirDotnet.Domain;

public class FlightAvailability
{
    public string FlightId { get; set; } = null!;
    public int SegmentNumber { get; set; }
    public DateOnly FlightDate { get; set; }
    public int EconomySeatsTaken { get; set; }
    public int BusinessSeatsTaken { get; set; }
    public int FirstclassSeatsTaken { get; set; }
}
