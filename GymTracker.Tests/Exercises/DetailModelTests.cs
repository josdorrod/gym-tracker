using FluentAssertions;
using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.Pages.Exercises;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class DetailModelTests
{
    private static GymTrackerDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GymTrackerDbContext(options);
    }

    [TestMethod]
    public async Task OnGetAsync_ExerciseExists_ReturnsPageWithExerciseAndSets()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Press Banca", MuscleGroup = "Pecho" };
        db.Exercises.Add(exercise);
        db.Sets.Add(new Set { Exercise = exercise, Weight = 80, Reps = 8, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks });
        await db.SaveChangesAsync();
        var model = new DetailModel(db);

        // Act
        var result = await model.OnGetAsync(exercise.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Exercise.Should().NotBeNull();
        model.Exercise.Name.Should().Be("Press Banca");
        model.Sets.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task OnGetAsync_ExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        using var db = CreateDb();
        var model = new DetailModel(db);

        // Act
        var result = await model.OnGetAsync(123);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
