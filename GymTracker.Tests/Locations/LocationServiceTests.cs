using FluentAssertions;
using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymTracker.Tests.Locations;

[TestClass]
public class LocationServiceTests
{
    [TestMethod]
    public async Task GetAllLocationsAsync_OrdenaPorNombre()
    {
        // Arrange
        using var db = CreateContext();
        db.Locations.AddRange(
            new Location { Name = "Zeus Box" },
            new Location { Name = "Atlas Gym" });
        await db.SaveChangesAsync();

        var service = new LocationService(db);

        // Act
        var result = await service.GetAllLocationsAsync();

        // Assert
        result.Select(location => location.Name).Should().ContainInOrder("Atlas Gym", "Zeus Box");
    }

    [TestMethod]
    public async Task CreateLocationAsync_GuardaValoresRecortados()
    {
        // Arrange
        using var db = CreateContext();
        var service = new LocationService(db);

        // Act
        var id = await service.CreateLocationAsync(new LocationEditDTO
        {
            Name = "  Centro Fit  "
        });

        // Assert
        id.Should().BeGreaterThan(0);
        var location = await db.Locations.FindAsync(id);
        location.Should().NotBeNull();
        location!.Name.Should().Be("Centro Fit");
    }

    [TestMethod]
    public async Task UpdateLocationAsync_CuandoNoExiste_RetornaFalse()
    {
        // Arrange
        using var db = CreateContext();
        var service = new LocationService(db);

        // Act
        var updated = await service.UpdateLocationAsync(999, new LocationEditDTO { Name = "Nueva" });

        // Assert
        updated.Should().BeFalse();
    }

    [TestMethod]
    public async Task DeleteLocationAsync_CuandoTieneSesiones_RetornaInUse()
    {
        // Arrange
        using var db = CreateContext();
        var location = new Location { Name = "Powerhouse" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        db.WorkoutSessions.Add(new WorkoutSession
        {
            LocationId = location.Id,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        });
        await db.SaveChangesAsync();

        var service = new LocationService(db);

        // Act
        var result = await service.DeleteLocationAsync(location.Id);

        // Assert
        result.Should().Be(LocationDeleteResult.InUse);
        (await db.Locations.FindAsync(location.Id)).Should().NotBeNull();
    }

    [TestMethod]
    public async Task DeleteLocationAsync_CuandoExisteYNoTieneSesiones_LaElimina()
    {
        // Arrange
        using var db = CreateContext();
        var location = new Location { Name = "North Gym" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        var service = new LocationService(db);

        // Act
        var result = await service.DeleteLocationAsync(location.Id);

        // Assert
        result.Should().Be(LocationDeleteResult.Deleted);
        (await db.Locations.FindAsync(location.Id)).Should().BeNull();
    }

    private static GymTrackerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymTrackerDbContext(options);
    }
}