using FluentAssertions;
using GymTracker.DTO;
using GymTracker.Pages.Exercises;
using GymTracker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class IndexModelTests
{
    [TestMethod]
    public async Task OnGetAsync_ConNoEjercicios_RetornaListaVacia()
    {
        // Arrange
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock
            .Setup(s => s.GetAllExercisesAsync())
            .ReturnsAsync(new List<ExerciseListDTO>());
        var model = new IndexModel(exerciseServiceMock.Object);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Exercises.Should().BeEmpty();
    }

    [TestMethod]
    public async Task OnGetAsync_ConEjercicios_AsignaLaListaRetornadaPorElServicio()
    {
        // Arrange
        var exercises = new List<ExerciseListDTO>
        {
            new ExerciseListDTO { Id = 1, Name = "Bíceps Curl",  MuscleGroup = "Brazo" },
            new ExerciseListDTO { Id = 2, Name = "Press Banca",  MuscleGroup = "Pecho" },
            new ExerciseListDTO { Id = 3, Name = "Zancada",      MuscleGroup = "Pierna" }
        };

        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock
            .Setup(s => s.GetAllExercisesAsync())
            .ReturnsAsync(exercises);
        var model = new IndexModel(exerciseServiceMock.Object);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Exercises.Should().HaveCount(3);
        model.Exercises.Select(e => e.Name).Should().BeInAscendingOrder();
    }
}
