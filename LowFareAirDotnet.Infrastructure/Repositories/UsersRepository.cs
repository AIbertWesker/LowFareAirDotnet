using LowFareAirDotnet.Domain;
using LowFareAirDotnet.Infrastructure.DbContexts;
using LowFareAirDotnet.Infrastructure.Models;
using LowFareAirDotnet.Logic.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LowFareAirDotnet.Infrastructure.Repositories;

internal class UsersRepository : IUsersRepository
{
    private readonly RelationalDbContext _context;
    public UsersRepository(RelationalDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByName(string username)
    {
        var user = await _context.Users.Where(x => x.Name == username).FirstOrDefaultAsync();
        if (user is null) return null;
        var result = new User
        {
            Id = user.Id,
            Name = user.Name,
            Password = user.Password
        };

        return result;
    }

    public async Task AddNewUser(string username, string password)
    {
        var user = new UserModel
        {
            Name = username,
            Password = password
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
