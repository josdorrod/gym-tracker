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
        model.ExerciseDto.Should().NotBeNull();
        model.ExerciseDto.Name.Should().Be("Press Banca");
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
    [TestMethod]
    public async Task OnPostDeleteAsync_DeletesExistingSet_Success()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Sentadilla", MuscleGroup = "Pierna" };
        db.Exercises.Add(exercise);
        var set = new Set { Exercise = exercise, Weight = 100, Reps = 5, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks };
        db.Sets.Add(set);
        await db.SaveChangesAsync();
        var model = new DetailModel(db);

        // Act
        var result = await model.OnPostDeleteAsync(set.Id);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        db.Sets.Where(s => s.ExerciseId == exercise.Id).Should().BeEmpty();
        db.Exercises.Find(exercise.Id).Should().NotBeNull();
    }

    [TestMethod]
    public async Task OnPostDeleteAsync_NonExistentSetId_ReturnsNotFound()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Press Militar", MuscleGroup = "Hombro" };
        db.Exercises.Add(exercise);
        var set = new Set { Exercise = exercise, Weight = 50, Reps = 10, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks };
        db.Sets.Add(set);
        await db.SaveChangesAsync();
        var model = new DetailModel(db);

        // Act
        var result = await model.OnPostDeleteAsync(set.Id + 999); // Non-existent setId

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        db.Sets.Find(set.Id).Should().NotBeNull();
        db.Exercises.Find(exercise.Id).Should().NotBeNull();
    }

    [TestMethod]
    public async Task OnPostDeleteAsync_MultipleDeletions_OnlyTargetedSetRemoved()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Remo", MuscleGroup = "Espalda" };
        db.Exercises.Add(exercise);
        var set1 = new Set { Exercise = exercise, Weight = 60, Reps = 12, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks };
        var set2 = new Set { Exercise = exercise, Weight = 65, Reps = 10, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks + 1 };
        var set3 = new Set { Exercise = exercise, Weight = 70, Reps = 8, CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks + 2 };
        db.Sets.AddRange(set1, set2, set3);
        await db.SaveChangesAsync();
        var model = new DetailModel(db);

        // Act & Assert
        (await model.OnPostDeleteAsync(set1.Id)).Should().BeOfType<RedirectToPageResult>();
        db.Sets.Where(s => s.ExerciseId == exercise.Id).Should().HaveCount(2);
        db.Sets.Find(set2.Id).Should().NotBeNull();
        db.Sets.Find(set3.Id).Should().NotBeNull();

        (await model.OnPostDeleteAsync(set2.Id)).Should().BeOfType<RedirectToPageResult>();
        db.Sets.Where(s => s.ExerciseId == exercise.Id).Should().HaveCount(1);
        db.Sets.Find(set3.Id).Should().NotBeNull();

        (await model.OnPostDeleteAsync(set3.Id)).Should().BeOfType<RedirectToPageResult>();
        db.Sets.Where(s => s.ExerciseId == exercise.Id).Should().BeEmpty();
        db.Exercises.Find(exercise.Id).Should().NotBeNull();
    }
}
