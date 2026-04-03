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
public class EditModelTests
{
    [TestMethod]
    public async Task OnGetAsync_EjercicioExiste_RetornaPaginaConDatosDelEjercicio()
    {
        // Arrange
        const int exerciseId = 1;
        var exerciseDto = new ExerciseCreateDTO
        {
            Name = "Sentadilla Libre",
            MuscleGroup = "Pierna",
            PlannedSets = 4,
            Instructions = "Bajar hasta 90 grados"
        };

        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock.Setup(s => s.GetExerciseByIdAsync(exerciseId)).ReturnsAsync(exerciseDto);
        var model = new EditModel(exerciseServiceMock.Object);

        // Act
        var result = await model.OnGetAsync(exerciseId);

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
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock.Setup(s => s.GetExerciseByIdAsync(It.IsAny<int>())).ReturnsAsync((ExerciseCreateDTO?)null);
        var model = new EditModel(exerciseServiceMock.Object);

        // Act
        var result = await model.OnGetAsync(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloValido_LlamaAlServicioYRedirigeAIndex()
    {
        // Arrange
        const int exerciseId = 3;
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock.Setup(s => s.UpdateExerciseAsync(exerciseId, It.IsAny<ExerciseCreateDTO>())).ReturnsAsync(true);
        var model = new EditModel(exerciseServiceMock.Object);
        model.ExerciseInput = new ExerciseCreateDTO
        {
            Name = "Press Banca Inclinado",
            MuscleGroup = "Pecho Superior",
            PlannedSets = 4,
            Instructions = "Agarre medio"
        };

        // Act
        var result = await model.OnPostAsync(exerciseId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>()
            .Which.PageName.Should().Be("Index");
        exerciseServiceMock.Verify(s => s.UpdateExerciseAsync(exerciseId, model.ExerciseInput), Times.Once);
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloInvalido_RetornaPaginaSinLlamarAlServicio()
    {
        // Arrange
        var exerciseServiceMock = new Mock<IExerciseService>();
        var model = new EditModel(exerciseServiceMock.Object);
        model.ModelState.AddModelError("ExerciseInput.Name", "El nombre es obligatorio");

        // Act
        var result = await model.OnPostAsync(1);

        // Assert
        result.Should().BeOfType<PageResult>();
        exerciseServiceMock.Verify(s => s.UpdateExerciseAsync(It.IsAny<int>(), It.IsAny<ExerciseCreateDTO>()), Times.Never);
    }

    [TestMethod]
    public async Task OnPostAsync_EjercicioNoExiste_RetornaNotFound()
    {
        // Arrange
        const int nonExistentId = 999;
        var exerciseServiceMock = new Mock<IExerciseService>();
        exerciseServiceMock.Setup(s => s.UpdateExerciseAsync(nonExistentId, It.IsAny<ExerciseCreateDTO>())).ReturnsAsync(false);
        var model = new EditModel(exerciseServiceMock.Object);
        model.ExerciseInput = new ExerciseCreateDTO { Name = "Curl Bíceps" };

        // Act
        var result = await model.OnPostAsync(nonExistentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
