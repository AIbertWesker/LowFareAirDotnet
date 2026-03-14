using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LowFareAirDotnet.Infrastructure.Models;

public class FlightAvailabilityModel
{
    public string FlightId { get; set; } = null!;
    public int SegmentNumber { get; set; }
    public DateOnly FlightDate { get; set; }
    public int EconomySeatsTaken { get; set; }
    public int BusinessSeatsTaken { get; set; }
    public int FirstclassSeatsTaken { get; set; }

    //NAWIGACJA
    public FlightModel Flight { get; set; } = null!;
}

//CREATE TABLE flightavailability(
//    flight_id VARCHAR(6) NOT NULL,
//    segment_number INTEGER NOT NULL,
//    flight_date DATE NOT NULL,
//    economy_seats_taken INTEGER DEFAULT 0,
//    business_seats_taken INTEGER DEFAULT 0,
//    firstclass_seats_taken INTEGER DEFAULT 0,
//    CONSTRAINT flightavail_pk PRIMARY KEY(flight_id, segment_number, flight_date),
//    CONSTRAINT flights_fk2 FOREIGN KEY(flight_id, segment_number)
//        REFERENCES flights(flight_id, segment_number)
//);