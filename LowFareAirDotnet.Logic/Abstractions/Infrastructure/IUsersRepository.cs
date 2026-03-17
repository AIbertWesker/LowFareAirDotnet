using LowFareAirDotnet.Domain;

namespace LowFareAirDotnet.Logic.Abstractions.Infrastructure;

public interface IUsersRepository
{
    Task AddNewUser(string username, string password);
    Task<User?> GetUserByName(string username);
}