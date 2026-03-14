namespace LowFareAirDotnet.Infrastructure.Models;

public class FlightModel
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

    //NAWIGACJA
    public ICollection<FlightAvailabilityModel> FlightAvailabilities { get; set; } = new List<FlightAvailabilityModel>();
}

//CREATE TABLE flights(
//    flight_id VARCHAR(6) NOT NULL,
//    segment_number INTEGER NOT NULL,
//    orig_airport CHAR(3),
//    depart_time TIME,
//    dest_airport CHAR(3),
//    arrive_time TIME,
//    meal CHAR(1),
//    flying_time DOUBLE PRECISION,
//    miles INTEGER,
//    aircraft VARCHAR(6),
//    CONSTRAINT flights_pk PRIMARY KEY(flight_id, segment_number),
//    CONSTRAINT meal_constraint CHECK(meal IN ('B','L','D','S'))
//);
