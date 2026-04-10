using FluentAssertions;
using GymTracker.DTO;
using GymTracker.Pages.Locations;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GymTracker.Tests.Locations;

[TestClass]
public class CreateModelTests
{
    [TestMethod]
    public async Task OnPostAsync_ModeloValido_CreaLaUbicacionYRetornaNoContent()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.CreateLocationAsync(It.IsAny<LocationEditDTO>())).ReturnsAsync(5);

        var model = new GymTracker.Pages.Locations.CreateModel(serviceMock.Object)
        {
            Input = new LocationEditDTO { Name = "Downtown Gym" }
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(service => service.CreateLocationAsync(model.Input), Times.Once);
    }

    [TestMethod]
    public async Task OnPostAsync_ModeloInvalido_RetornaPageSinCrear()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        var model = new GymTracker.Pages.Locations.CreateModel(serviceMock.Object);
        model.ModelState.AddModelError("Input.Name", "El nombre es obligatorio");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        serviceMock.Verify(service => service.CreateLocationAsync(It.IsAny<LocationEditDTO>()), Times.Never);
    }
}