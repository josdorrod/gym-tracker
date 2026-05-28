using FluentAssertions;
using GymTracker.DTO;
using GymTracker.Pages.Exercises;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class CreateModelTests
{
    [TestMethod]
    public void OnGet_NoLanzaExcepcion()
    {
        // Arrange
        var exerciseServiceMock = new Mock<IExerciseService>();
        var model = new CreateModel(exerciseServiceMock.Object);

        // Act & Assert
        model.Invoking(m => m.OnGet()).Should().NotThrow();
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_LlamaAlServicioYRedirigEADetalle()
    {
        // Arrange
        const int newExerciseId = 42;
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock
            .Setup(s => s.CreateExerciseAsync(It.IsAny<ExerciseCreateDTO>()))
            .ReturnsAsync(newExerciseId);

        var model = new CreateModel(exerciseServiceMock.Object);
        model.Input = new ExerciseCreateDTO { Name = "Sentadilla Libre", MuscleGroup = "Pierna" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        exerciseServiceMock.Verify(s => s.CreateExerciseAsync(model.Input), Times.Once);
        result.Should().BeOfType<RedirectToPageResult>()
            .Which.PageName.Should().Be("Detail");
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloInvalido_RetornaPaginaSinLlamarAlServicio()
    {
        // Arrange
        var exerciseServiceMock = new Mock<IExerciseService>();
        var model = new CreateModel(exerciseServiceMock.Object);
        model.ModelState.AddModelError("Input.Name", "El nombre es obligatorio");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        exerciseServiceMock.Verify(s => s.CreateExerciseAsync(It.IsAny<ExerciseCreateDTO>()), Times.Never);
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_ElIdRetornadoPorElServicioEstaEnLaRedireccion()
    {
        // Arrange
        const int newExerciseId = 7;
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock
            .Setup(s => s.CreateExerciseAsync(It.IsAny<ExerciseCreateDTO>()))
            .ReturnsAsync(newExerciseId);

        var model = new CreateModel(exerciseServiceMock.Object);
        model.Input = new ExerciseCreateDTO { Name = "Peso Muerto" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirect.RouteValues.Should().ContainKey("id");
        redirect.RouteValues!["id"].Should().Be(newExerciseId);
    }
}
