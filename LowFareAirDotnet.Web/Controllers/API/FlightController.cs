using System.Security.Claims;
using LowFareAirDotnet.Infrastructure.DbContexts;
using LowFareAirDotnet.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowFareAirDotnet.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly RelationalDbContext _db;

        public FlightController(RelationalDbContext db)
        {
            _db = db;
        }

        public enum SeatClass
        {
            Economy = 0,
            Business = 1,
            Firstclass = 2
        }

        public sealed record SearchFlightsResponse(
            string FlightId,
            int SegmentNumber,
            DateOnly FlightDate,
            string? OrigAirport,
            string? DestAirport,
            TimeOnly? DepartTime,
            TimeOnly? ArriveTime,
            int EconomySeatsTaken,
            int BusinessSeatsTaken,
            int FirstclassSeatsTaken
        );

        public sealed record FlightDetailsResponse(
            string FlightId,
            int SegmentNumber,
            DateOnly FlightDate,
            string? OrigAirport,
            string? OrigCityName,
            string? OrigCountry,
            string? OrigCountryCode,
            string? DestAirport,
            string? DestCityName,
            string? DestCountry,
            string? DestCountryCode,
            TimeOnly? DepartTime,
            TimeOnly? ArriveTime,
            string? Meal,
            string? MealDescription,
            double? FlyingTime,
            int? Miles,
            string? Aircraft,
            int EconomySeatsTaken,
            int BusinessSeatsTaken,
            int FirstclassSeatsTaken,
            int TotalSeatsTaken
        );

        public sealed record BookFlightRequest(
            string FlightId,
            int SegmentNumber,
            DateOnly FlightDate,
            SeatClass Class
        );

        public sealed record BookFlightResponse(
            int FlightHistoryId
        );

        public sealed record MyBookedFlightResponse(
            int FlightHistoryId,
            string Username,
            string FlightId,
            int? SegmentNumber,
            string OrigAirport,
            string? OrigCityName,
            string? OrigCountryCode,
            string DestAirport,
            string? DestCityName,
            string? DestCountryCode,
            string? BeginDate,
            string? Class,
            TimeOnly? DepartTime,
            TimeOnly? ArriveTime,
            string? Aircraft,
            int TotalBookedByUser
        );

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> MyBookings(CancellationToken cancellationToken = default)
        {
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            var userBookings = _db.FlightHistories
                .AsNoTracking()
                .Where(h => h.Username == username)
                .AsQueryable();

            var totalBookedByUser = await userBookings.CountAsync(cancellationToken);

            var bookings = await (
                from h in userBookings
                join f in _db.Flights.AsNoTracking()
                    on new { h.FlightId, h.OrigAirport, h.DestAirport }
                    equals new { f.FlightId, OrigAirport = f.OrigAirport!, DestAirport = f.DestAirport! } into flights
                from f in flights.DefaultIfEmpty()
                join oc in _db.Cities.AsNoTracking()
                    on h.OrigAirport equals oc.Airport into origCities
                from oc in origCities.DefaultIfEmpty()
                join dc in _db.Cities.AsNoTracking()
                    on h.DestAirport equals dc.Airport into destCities
                from dc in destCities.DefaultIfEmpty()
                orderby h.Id descending
                select new MyBookedFlightResponse(
                    h.Id,
                    h.Username,
                    h.FlightId,
                    f != null ? f.SegmentNumber : null,
                    h.OrigAirport,
                    oc != null ? oc.CityName : null,
                    oc != null ? oc.CountryIsoCode : null,
                    h.DestAirport,
                    dc != null ? dc.CityName : null,
                    dc != null ? dc.CountryIsoCode : null,
                    h.BeginDate,
                    h.Class,
                    f != null ? f.DepartTime : null,
                    f != null ? f.ArriveTime : null,
                    f != null ? f.Aircraft : null,
                    totalBookedByUser
                )
            ).ToListAsync(cancellationToken);

            return Ok(bookings);
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details(
            [FromQuery] string flightId,
            [FromQuery] int segmentNumber,
            [FromQuery] DateOnly flightDate,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(flightId))
                return BadRequest("Missing query param: flightId");

            var normalizedFlightId = flightId.Trim().ToUpperInvariant();

            var details = await (
                from fa in _db.FlightAvailabilities.AsNoTracking()
                join f in _db.Flights.AsNoTracking()
                    on new { fa.FlightId, fa.SegmentNumber }
                    equals new { f.FlightId, f.SegmentNumber }
                join oc in _db.Cities.AsNoTracking()
                    on f.OrigAirport equals oc.Airport into origCities
                from oc in origCities.DefaultIfEmpty()
                join dc in _db.Cities.AsNoTracking()
                    on f.DestAirport equals dc.Airport into destCities
                from dc in destCities.DefaultIfEmpty()
                where fa.FlightId == normalizedFlightId
                    && fa.SegmentNumber == segmentNumber
                    && fa.FlightDate == flightDate
                select new FlightDetailsResponse(
                    f.FlightId,
                    f.SegmentNumber,
                    fa.FlightDate,
                    f.OrigAirport,
                    oc != null ? oc.CityName : null,
                    oc != null ? oc.Country : null,
                    oc != null ? oc.CountryIsoCode : null,
                    f.DestAirport,
                    dc != null ? dc.CityName : null,
                    dc != null ? dc.Country : null,
                    dc != null ? dc.CountryIsoCode : null,
                    f.DepartTime,
                    f.ArriveTime,
                    f.Meal,
                    MapMealCodeToDescription(f.Meal),
                    f.FlyingTime,
                    f.Miles,
                    f.Aircraft,
                    fa.EconomySeatsTaken,
                    fa.BusinessSeatsTaken,
                    fa.FirstclassSeatsTaken,
                    fa.EconomySeatsTaken + fa.BusinessSeatsTaken + fa.FirstclassSeatsTaken
                )
            ).FirstOrDefaultAsync(cancellationToken);

            if (details is null)
                return NotFound("Flight details not found for given (flightId, segmentNumber, flightDate)");

            return Ok(details);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? fromAirport,
            [FromQuery] string? toAirport,
            [FromQuery] DateOnly? date,
            [FromQuery] int take = 6000,
            CancellationToken cancellationToken = default)
        {
            take = Math.Clamp(take, 1, 6000);

            var q = _db.FlightAvailabilities
                .AsNoTracking()
                .Select(fa => new { fa, fa.Flight })
                .AsQueryable();

            if (date.HasValue)
            {
                q = q.Where(x => x.fa.FlightDate == date.Value);
            }
             
            if (!string.IsNullOrWhiteSpace(fromAirport))
            {
                var from = fromAirport.Trim().ToUpperInvariant();
                q = q.Where(x => x.Flight.OrigAirport != null && x.Flight.OrigAirport.ToUpper() == from);
            }

            if (!string.IsNullOrWhiteSpace(toAirport))
            {
                var to = toAirport.Trim().ToUpperInvariant();
                q = q.Where(x => x.Flight.DestAirport != null && x.Flight.DestAirport.ToUpper() == to);
            }

            var results = await q
                .OrderBy(x => x.Flight.DepartTime)
                .Take(take)
                .Select(x => new SearchFlightsResponse(
                    x.Flight.FlightId,
                    x.fa.SegmentNumber,
                    x.fa.FlightDate,
                    x.Flight.OrigAirport,
                    x.Flight.DestAirport,
                    x.Flight.DepartTime,
                    x.Flight.ArriveTime,
                    x.fa.EconomySeatsTaken,
                    x.fa.BusinessSeatsTaken,
                    x.fa.FirstclassSeatsTaken
                ))
                .ToListAsync(cancellationToken);

            return Ok(results);
        }

        [Authorize]
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookFlightRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.FlightId))
                return BadRequest("FlightId is required");

            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var availability = await _db.FlightAvailabilities
                .FirstOrDefaultAsync(x => x.FlightId == request.FlightId
                    && x.SegmentNumber == request.SegmentNumber
                    && x.FlightDate == request.FlightDate, cancellationToken);

            if (availability is null)
                return NotFound("Flight availability not found for given (flightId, segmentNumber, flightDate)");

            var flight = await _db.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FlightId == request.FlightId && x.SegmentNumber == request.SegmentNumber, cancellationToken);

            if (flight is null)
                return NotFound("Flight segment not found for given (flightId, segmentNumber)");

            IncrementTakenSeats(availability, request.Class);

            var history = new FlightHistoryModel
            {
                Username = username,
                FlightId = request.FlightId,
                OrigAirport = flight.OrigAirport ?? string.Empty,
                DestAirport = flight.DestAirport ?? string.Empty,
                BeginDate = request.FlightDate.ToString("yyyy-MM-dd"),
                Class = request.Class.ToString()
            };

            _db.FlightHistories.Add(history);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Conflict("Concurrent update detected. Please retry.");
            }

            return Ok(new BookFlightResponse(history.Id));
        }

        private static void IncrementTakenSeats(FlightAvailabilityModel availability, SeatClass seatClass)
        {
            switch (seatClass)
            {
                case SeatClass.Economy:
                    availability.EconomySeatsTaken++;
                    break;
                case SeatClass.Business:
                    availability.BusinessSeatsTaken++;
                    break;
                case SeatClass.Firstclass:
                    availability.FirstclassSeatsTaken++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seatClass), seatClass, null);
            }
        }

        private static string? MapMealCodeToDescription(string? mealCode) => mealCode switch
        {
            "B" => "Breakfast",
            "L" => "Lunch",
            "D" => "Dinner",
            "S" => "Snack",
            _ => null
        };
    }
}
