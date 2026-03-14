namespace LowFareAirDotnet.Infrastructure.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
}


//CREATE TABLE users(
//    id SERIAL PRIMARY KEY,
//    username VARCHAR(40) NOT NULL UNIQUE,
//    password VARCHAR(20)
//);
