using FluentAssertions;
using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.Pages.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class IndexModelTests
{
    private static GymTrackerDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GymTrackerDbContext(options);
    }

    [TestMethod]
    public async Task OnGetAsync_ConNoEjercicios_RetornaListaVacia()
    {
        // Arrange
        using var db = CreateDb();
        var model = new IndexModel(db);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Exercises.Should().BeEmpty();
    }

    [TestMethod]
    public async Task OnGetAsync_ConEjercicios_RetornaOrdenadosPorNombre()
    {
        // Arrange
        using var db = CreateDb();
        db.Exercises.AddRange(
            new Exercise { Name = "Zancada", MuscleGroup = "Pierna" },
            new Exercise { Name = "Bíceps Curl", MuscleGroup = "Brazo" },
            new Exercise { Name = "Press Banca", MuscleGroup = "Pecho" }
        );
        await db.SaveChangesAsync();
        var model = new IndexModel(db);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Exercises.Should().HaveCount(3);
        model.Exercises.Select(e => e.Name).Should().BeInAscendingOrder();
    }
}
