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
public class CreateModelTests
{
    private static GymTrackerDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<GymTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GymTrackerDbContext(options);
    }

    [TestMethod]
    public void OnGet_NoLanzaExcepcion()
    {
        // Arrange
        using var db = CreateDb();
        var model = new CreateModel(db);

        // Act & Assert
        model.Invoking(m => m.OnGet()).Should().NotThrow();
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_GuardaEjercicioYRedirigEADetalle()
    {
        // Arrange
        using var db = CreateDb();
        var model = new CreateModel(db);
        model.Input = new Exercise { Name = "Sentadilla Libre", MuscleGroup = "Pierna" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        db.Exercises.Should().HaveCount(1);
        result.Should().BeOfType<RedirectToPageResult>()
            .Which.PageName.Should().Be("Detail");
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloInvalido_RetornaPaginaSinGuardar()
    {
        // Arrange
        using var db = CreateDb();
        var model = new CreateModel(db);
        model.ModelState.AddModelError("Input.Name", "El nombre es obligatorio");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        db.Exercises.Should().BeEmpty();
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_ElIdDelEjercicioEstaEnLaRedireccion()
    {
        // Arrange
        using var db = CreateDb();
        var model = new CreateModel(db);
        model.Input = new Exercise { Name = "Peso Muerto" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirect.RouteValues.Should().ContainKey("id");
        redirect.RouteValues!["id"].Should().Be(model.Input.Id);
    }
}
