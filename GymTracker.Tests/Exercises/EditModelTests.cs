using FluentAssertions;
using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Pages.Exercises;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class EditModelTests
{
    private static GymTrackerDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GymTrackerDbContext(options);
    }

    [TestMethod]
    public async Task OnGetAsync_EjercicioExiste_RetornaPaginaConDatosDelEjercicio()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Sentadilla Libre", MuscleGroup = "Pierna", PlannedSets = 4, Instructions = "Bajar hasta 90 grados" };
        db.Exercises.Add(exercise);
        await db.SaveChangesAsync();
        var model = new EditModel(db);

        // Act
        var result = await model.OnGetAsync(exercise.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ExerciseInput.Name.Should().Be("Sentadilla Libre");
        model.ExerciseInput.MuscleGroup.Should().Be("Pierna");
        model.ExerciseInput.PlannedSets.Should().Be(4);
        model.ExerciseInput.Instructions.Should().Be("Bajar hasta 90 grados");
    }

    [TestMethod]
    public async Task OnGetAsync_EjercicioNoExiste_RetornaNotFound()
    {
        // Arrange
        using var db = CreateDb();
        var model = new EditModel(db);

        // Act
        var result = await model.OnGetAsync(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_ActualizaEjercicioYRedirigeAIndex()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Press Banca", MuscleGroup = "Pecho" };
        db.Exercises.Add(exercise);
        await db.SaveChangesAsync();
        var model = new EditModel(db);
        model.ExerciseInput = new ExerciseInputDTO { Name = "Press Banca Inclinado", MuscleGroup = "Pecho Superior", PlannedSets = 4, Instructions = "Agarre medio" };

        // Act
        var result = await model.OnPostAsync(exercise.Id);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>()
            .Which.PageName.Should().Be("Index");
        var updated = await db.Exercises.FindAsync(exercise.Id);
        updated!.Name.Should().Be("Press Banca Inclinado");
        updated.MuscleGroup.Should().Be("Pecho Superior");
        updated.PlannedSets.Should().Be(4);
        updated.Instructions.Should().Be("Agarre medio");
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloInvalido_RetornaPaginaSinGuardar()
    {
        // Arrange
        using var db = CreateDb();
        var exercise = new Exercise { Name = "Peso Muerto", MuscleGroup = "Espalda" };
        db.Exercises.Add(exercise);
        await db.SaveChangesAsync();
        var model = new EditModel(db);
        model.ModelState.AddModelError("ExerciseInput.Name", "El nombre es obligatorio");

        // Act
        var result = await model.OnPostAsync(exercise.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        var unchanged = await db.Exercises.FindAsync(exercise.Id);
        unchanged!.Name.Should().Be("Peso Muerto");
    }

    [TestMethod]
    public async Task OnPostAsync_EjercicioNoExiste_RetornaPagina()
    {
        // Arrange
        using var db = CreateDb();
        var model = new EditModel(db);
        model.ExerciseInput = new ExerciseInputDTO { Name = "Curl Bíceps" };

        // Act
        var result = await model.OnPostAsync(999);

        // Assert
        result.Should().BeOfType<PageResult>();
    }
}
