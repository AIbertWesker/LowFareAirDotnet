using LowFareAirDotnet.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowFareAirDotnet.Web.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public sealed class CitiesController : ControllerBase
{
    private readonly RelationalDbContext _context; //KIEDYS TO PRZENIESC DO INSFRASTRUKTURY (ale mi sie teraz nie chce)

    public CitiesController(RelationalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> CitiesList()
    {
        var cities = await _context.Cities.Select(x => new { Code = x.Airport, Name = x.CityName, CountryCode = x.CountryIsoCode}).ToListAsync();
        return Ok(cities);
    }
}
