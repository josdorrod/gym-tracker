using FluentAssertions;
using GymTracker.DTO;
using GymTracker.Pages.Exercises;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GymTracker.Tests.Exercises;

[TestClass]
public class DetailModelTests
{
    private static (Mock<IExerciseService>, Mock<ISetService>, Mock<IWorkoutSessionService>, DetailModel) CreateSut()
    {
        var exerciseServiceMock = new Mock<IExerciseService>();
        var setServiceMock = new Mock<ISetService>();
        var sessionServiceMock = new Mock<IWorkoutSessionService>();
        var model = new DetailModel(exerciseServiceMock.Object, setServiceMock.Object, sessionServiceMock.Object);
        model.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return (exerciseServiceMock, setServiceMock, sessionServiceMock, model);
    }

    [TestMethod]
    public async Task OnGetAsync_ExerciseExists_ReturnsPageWithExerciseAndSets()
    {
        // Arrange
        const int exerciseId = 1;
        var exerciseDto = new ExerciseCreateDTO { Name = "Press Banca", MuscleGroup = "Pecho" };
        var sets = new List<SetDetailListDTO>
        {
            new SetDetailListDTO { Id = 10, Weight = 80, Reps = 8, SetNumber = 1 }
        };

        var (exerciseServiceMock, setServiceMock, sessionServiceMock, model) = CreateSut();
        exerciseServiceMock.Setup(s => s.GetExerciseByIdAsync(exerciseId)).ReturnsAsync(exerciseDto);
        setServiceMock.Setup(s => s.GetAllSetsAsync(exerciseId)).ReturnsAsync(sets);
        setServiceMock.Setup(s => s.GetTodaySetCount(exerciseId)).ReturnsAsync(1);
        sessionServiceMock.Setup(s => s.GetActiveSessionAsync()).ReturnsAsync((WorkoutSessionActiveDTO?)null);

        // Act
        var result = await model.OnGetAsync(exerciseId);

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
        var (exerciseServiceMock, _, _, model) = CreateSut();
        exerciseServiceMock.Setup(s => s.GetExerciseByIdAsync(It.IsAny<int>())).ReturnsAsync((ExerciseCreateDTO?)null);

        // Act
        var result = await model.OnGetAsync(123);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task OnPostDeleteAsync_DeletesExistingSet_Success()
    {
        // Arrange
        const int setId = 5;
        var (_, setServiceMock, _, model) = CreateSut();
        setServiceMock.Setup(s => s.DeleteSetAsync(setId)).ReturnsAsync(true);

        // Act
        var result = await model.OnPostDeleteAsync(setId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        setServiceMock.Verify(s => s.DeleteSetAsync(setId), Times.Once);
    }

    [TestMethod]
    public async Task OnPostDeleteAsync_NonExistentSetId_ReturnsNotFound()
    {
        // Arrange
        const int nonExistentSetId = 9999;
        var (_, setServiceMock, _, model) = CreateSut();
        setServiceMock.Setup(s => s.DeleteSetAsync(nonExistentSetId)).ReturnsAsync(false);

        // Act
        var result = await model.OnPostDeleteAsync(nonExistentSetId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task OnPostDeleteAsync_MultipleDeletions_EachCallPassesCorrectSetId()
    {
        // Arrange
        const int setId1 = 1;
        const int setId2 = 2;
        const int setId3 = 3;

        var (_, setServiceMock, _, model) = CreateSut();
        setServiceMock.Setup(s => s.DeleteSetAsync(It.IsAny<int>())).ReturnsAsync(true);

        // Act & Assert
        (await model.OnPostDeleteAsync(setId1)).Should().BeOfType<RedirectToPageResult>();
        (await model.OnPostDeleteAsync(setId2)).Should().BeOfType<RedirectToPageResult>();
        (await model.OnPostDeleteAsync(setId3)).Should().BeOfType<RedirectToPageResult>();

        setServiceMock.Verify(s => s.DeleteSetAsync(setId1), Times.Once);
        setServiceMock.Verify(s => s.DeleteSetAsync(setId2), Times.Once);
        setServiceMock.Verify(s => s.DeleteSetAsync(setId3), Times.Once);
    }

    [TestMethod]
    public async Task OnPostEndSessionAsync_CierraSesionActivaYRedirige()
    {
        const int exerciseId = 7;
        var (_, _, sessionServiceMock, model) = CreateSut();
        sessionServiceMock.Setup(s => s.CloseActiveSessionAsync()).ReturnsAsync(true);

        var result = await model.OnPostEndSessionAsync(exerciseId);

        result.Should().BeOfType<RedirectToPageResult>();
        sessionServiceMock.Verify(s => s.CloseActiveSessionAsync(), Times.Once);
    }
}
