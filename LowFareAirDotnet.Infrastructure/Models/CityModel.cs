namespace LowFareAirDotnet.Infrastructure.Models;

public class CityModel
{
    public int CityId { get; set; }
    public required string CityName { get; set; }
    public required string Country { get; set; }
    public string? Airport { get; set; }
    public string? Language { get; set; }
    public string? CountryIsoCode { get; set; }
}

//CREATE TABLE cities(
//    city_id SERIAL PRIMARY KEY,
//    city_name VARCHAR(24) NOT NULL,
//    country VARCHAR(26) NOT NULL,
//    airport VARCHAR(26),
//    language VARCHAR(16),
//    country_iso_code CHAR(2)
//);