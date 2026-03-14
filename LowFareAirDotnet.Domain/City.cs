namespace LowFareAirDotnet.Domain;

public class City
{
    public int CityId { get; set; }
    public required string CityName { get; set; }
    public required string Country { get; set; }
    public string? Airport { get; set; }
    public string? Language { get; set; }
    public string? CountryIsoCode { get; set; }
}
