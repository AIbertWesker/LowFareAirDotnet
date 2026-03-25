using LowFareAirDotnet.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LowFareAirDotnet.Infrastructure.DbContexts;

public class RelationalDbContext : DbContext
{
    public RelationalDbContext(DbContextOptions<RelationalDbContext> options)
    : base(options)
    {

    }

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<CityModel> Cities => Set<CityModel>();
    public DbSet<FlightModel> Flights => Set<FlightModel>();
    public DbSet<FlightAvailabilityModel> FlightAvailabilities => Set<FlightAvailabilityModel>();
    public DbSet<FlightHistoryModel> FlightHistories => Set<FlightHistoryModel>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).IsRequired().HasColumnName("username");
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Password).HasColumnName("password");
        });

        modelBuilder.Entity<CityModel>(entity =>
        {
            entity.ToTable("cities");
            entity.HasKey(x => x.CityId);

            entity.Property(x => x.CityId)
                .HasColumnName("city_id");

            entity.Property(x => x.CityName)
                .HasColumnName("city_name")
                .IsRequired()
                .HasMaxLength(24);

            entity.Property(x => x.Country)
                .HasColumnName("country")
                .IsRequired()
                .HasMaxLength(26);

            entity.Property(x => x.Airport)
                .HasColumnName("airport")
                .HasMaxLength(26);

            entity.Property(x => x.Language)
                .HasColumnName("language")
                .HasMaxLength(16);

            entity.Property(x => x.CountryIsoCode)
                .HasColumnName("country_iso_code")
                .HasMaxLength(2)
                .IsFixedLength();
        });

        modelBuilder.Entity<FlightModel>(entity =>
        {
            entity.ToTable("flights");
            entity.HasKey(x => new { x.FlightId, x.SegmentNumber })
                .HasName("flights_pk");

            entity.Property(x => x.FlightId)
                .HasColumnName("flight_id")
                .HasMaxLength(6)
                .IsRequired();

            entity.Property(x => x.SegmentNumber)
                .HasColumnName("segment_number");

            entity.Property(x => x.OrigAirport)
                .HasColumnName("orig_airport")
                .HasMaxLength(3)
                .IsFixedLength();

            entity.Property(x => x.DepartTime)
                .HasColumnName("depart_time");

            entity.Property(x => x.DestAirport)
                .HasColumnName("dest_airport")
                .HasMaxLength(3)
                .IsFixedLength();

            entity.Property(x => x.ArriveTime)
                .HasColumnName("arrive_time");

            entity.Property(x => x.Meal)
                .HasColumnName("meal")
                .HasMaxLength(1)
                .IsFixedLength();

            entity.Property(x => x.FlyingTime)
                .HasColumnName("flying_time");

            entity.Property(x => x.Miles)
                .HasColumnName("miles");

            entity.Property(x => x.Aircraft)
                .HasColumnName("aircraft")
                .HasMaxLength(6);

            entity.ToTable(t =>
                t.HasCheckConstraint("meal_constraint", "meal IN ('B','L','D','S')"));
        });

        modelBuilder.Entity<FlightAvailabilityModel>(entity =>
        {
            entity.ToTable("flightavailability");
            entity.HasKey(x => new { x.FlightId, x.SegmentNumber, x.FlightDate })
                .HasName("flightavail_pk");

            entity.Property(x => x.FlightId)
                .HasColumnName("flight_id")
                .HasMaxLength(6)
                .IsRequired();

            entity.Property(x => x.SegmentNumber)
                .HasColumnName("segment_number");

            entity.Property(x => x.FlightDate)
                .HasColumnName("flight_date");

            entity.Property(x => x.EconomySeatsTaken)
                .HasColumnName("economy_seats_taken")
                .HasDefaultValue(0);

            entity.Property(x => x.BusinessSeatsTaken)
                .HasColumnName("business_seats_taken")
                .HasDefaultValue(0);

            entity.Property(x => x.FirstclassSeatsTaken)
                .HasColumnName("firstclass_seats_taken")
                .HasDefaultValue(0);

            entity.HasOne(x => x.Flight)
                .WithMany(x => x.FlightAvailabilities)
                .HasForeignKey(x => new { x.FlightId, x.SegmentNumber })
                .HasConstraintName("flights_fk2");
        });

        modelBuilder.Entity<FlightHistoryModel>(entity =>
        {
            entity.ToTable("flighthistory");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Username)
                .HasColumnName("username")
                .HasMaxLength(26)
                .IsRequired();

            entity.Property(x => x.FlightId)
                .HasColumnName("flight_id")
                .HasMaxLength(6)
                .IsRequired();

            entity.Property(x => x.OrigAirport)
                .HasColumnName("orig_airport")
                .HasMaxLength(3)
                .IsRequired()
                .IsFixedLength();

            entity.Property(x => x.DestAirport)
                .HasColumnName("dest_airport")
                .HasMaxLength(3)
                .IsRequired()
                .IsFixedLength();

            entity.Property(x => x.BeginDate)
                .HasColumnName("begin_date")
                .HasMaxLength(12);

            entity.Property(x => x.Class)
                .HasColumnName("class")
                .HasMaxLength(12);
        });
    }
}
