namespace LowFareAirDotnet.Infrastructure.Models;

public class FlightHistoryModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FlightId { get; set; } = null!;
    public string OrigAirport { get; set; } = null!;
    public string DestAirport { get; set; } = null!;
    public string? BeginDate { get; set; }
    public string? Class { get; set; }
}

//CREATE TABLE flighthistory(
//    id SERIAL PRIMARY KEY,
//    username VARCHAR(26) NOT NULL,
//    flight_id VARCHAR(6) NOT NULL,
//    orig_airport CHAR(3) NOT NULL,
//    dest_airport CHAR(3) NOT NULL,
//    begin_date VARCHAR(12),
//    class VARCHAR(12)
//);