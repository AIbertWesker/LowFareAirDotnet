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
            Economy,
            Business,
            Firstclass
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

        public sealed record BookFlightRequest(
            string FlightId,
            int SegmentNumber,
            DateOnly FlightDate,
            SeatClass Class
        );

        public sealed record BookFlightResponse(
            int FlightHistoryId
        );

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? fromAirport,
            [FromQuery] string? toAirport,
            [FromQuery] DateOnly? date,
            [FromQuery] int take = 50,
            CancellationToken cancellationToken = default)
        {
            if (date is null)
                return BadRequest("Missing query param: date (YYYY-MM-DD)");

            take = Math.Clamp(take, 1, 200);

            var q = _db.FlightAvailabilities
                .AsNoTracking()
                .Where(fa => fa.FlightDate == date.Value)
                .Select(fa => new { fa, fa.Flight })
                .AsQueryable();

            var xd = await q.ToListAsync();
             
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
                    x.fa.FlightId,
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
    }
}
