using FluentAssertions;
using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymTracker.Tests.WorkoutSessions;

[TestClass]
public class WorkoutSessionServiceTests
{
    [TestMethod]
    public async Task GetActiveSessionAsync_CierraSesionesAbiertasDeDiasAnteriores()
    {
        using var db = CreateContext();
        var location = new Location { Name = "North Gym" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        var yesterday = DateTime.UtcNow.Date.AddDays(-1).AddHours(10);
        var staleSession = new WorkoutSession
        {
            LocationId = location.Id,
            StartTime = yesterday,
            EndTime = null
        };

        db.WorkoutSessions.Add(staleSession);
        await db.SaveChangesAsync();

        var service = new WorkoutSessionService(db);

        var activeSession = await service.GetActiveSessionAsync();

        activeSession.Should().BeNull();
        var savedSession = await db.WorkoutSessions.SingleAsync();
        savedSession.EndTime.Should().Be(savedSession.StartTime.Date.AddDays(1).AddTicks(-1));
    }

    [TestMethod]
    public async Task CreateSessionAsync_CuandoExisteSesionActivaHoy_ReutilizaLaMasReciente()
    {
        using var db = CreateContext();
        var location = new Location { Name = "North Gym" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        var olderSession = new WorkoutSession
        {
            LocationId = location.Id,
            StartTime = DateTime.UtcNow.Date.AddHours(8),
            EndTime = null
        };
        var latestSession = new WorkoutSession
        {
            LocationId = location.Id,
            StartTime = DateTime.UtcNow.Date.AddHours(11),
            EndTime = null
        };

        db.WorkoutSessions.AddRange(olderSession, latestSession);
        await db.SaveChangesAsync();

        var service = new WorkoutSessionService(db);

        var sessionId = await service.CreateSessionAsync(new WorkoutSessionCreateDTO
        {
            LocationId = location.Id
        });

        sessionId.Should().Be(latestSession.Id);
        (await db.WorkoutSessions.CountAsync()).Should().Be(2);
    }

    [TestMethod]
    public async Task CloseActiveSessionAsync_CuandoHaySesionActiva_LaCierra()
    {
        using var db = CreateContext();
        var location = new Location { Name = "North Gym" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        var session = new WorkoutSession
        {
            LocationId = location.Id,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        db.WorkoutSessions.Add(session);
        await db.SaveChangesAsync();

        var service = new WorkoutSessionService(db);

        var result = await service.CloseActiveSessionAsync();

        result.Should().BeTrue();
        var savedSession = await db.WorkoutSessions.SingleAsync();
        savedSession.EndTime.Should().NotBeNull();
        savedSession.EndTime.Should().BeOnOrAfter(savedSession.StartTime);
    }

    private static GymTrackerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymTrackerDbContext(options);
    }
}
