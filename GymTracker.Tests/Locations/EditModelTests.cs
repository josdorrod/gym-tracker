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
public class EditModelTests
{
    [TestMethod]
    public async Task OnGetAsync_CuandoExiste_CargaLosDatos()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.GetLocationForEditAsync(3)).ReturnsAsync(new LocationEditDTO
        {
            Name = "Arena Club"
        });

        var model = new GymTracker.Pages.Locations.EditModel(serviceMock.Object);

        // Act
        var result = await model.OnGetAsync(3);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Input.Name.Should().Be("Arena Club");
    }

    [TestMethod]
    public async Task OnPostAsync_CuandoEsValido_ActualizaYRetornaNoContent()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.UpdateLocationAsync(3, It.IsAny<LocationEditDTO>())).ReturnsAsync(true);

        var model = new GymTracker.Pages.Locations.EditModel(serviceMock.Object)
        {
            Input = new LocationEditDTO { Name = "Arena Club" }
        };

        // Act
        var result = await model.OnPostAsync(3);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(service => service.UpdateLocationAsync(3, model.Input), Times.Once);
    }

    [TestMethod]
    public async Task OnPostAsync_CuandoNoExiste_RetornaNotFound()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.UpdateLocationAsync(44, It.IsAny<LocationEditDTO>())).ReturnsAsync(false);

        var model = new GymTracker.Pages.Locations.EditModel(serviceMock.Object)
        {
            Input = new LocationEditDTO { Name = "Ghost Gym" }
        };

        // Act
        var result = await model.OnPostAsync(44);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}