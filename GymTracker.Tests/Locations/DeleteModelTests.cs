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
public class DeleteModelTests
{
    [TestMethod]
    public async Task OnGetAsync_CuandoExiste_RetornaPaginaConLaUbicacion()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.GetLocationByIdAsync(8)).ReturnsAsync(new LocationListDTO
        {
            Id = 8,
            Name = "Iron Temple"
        });

        var model = new GymTracker.Pages.Locations.DeleteModel(serviceMock.Object);

        // Act
        var result = await model.OnGetAsync(8);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Location.Name.Should().Be("Iron Temple");
    }

    [TestMethod]
    public async Task OnPostAsync_CuandoSeElimina_RetornaNoContent()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.DeleteLocationAsync(8)).ReturnsAsync(LocationDeleteResult.Deleted);

        var model = new GymTracker.Pages.Locations.DeleteModel(serviceMock.Object);

        // Act
        var result = await model.OnPostAsync(8);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task OnPostAsync_CuandoEstaEnUso_RetornaPaginaConError()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.DeleteLocationAsync(8)).ReturnsAsync(LocationDeleteResult.InUse);
        serviceMock.Setup(service => service.GetLocationByIdAsync(8)).ReturnsAsync(new LocationListDTO
        {
            Id = 8,
            Name = "Iron Temple"
        });

        var model = new GymTracker.Pages.Locations.DeleteModel(serviceMock.Object);

        // Act
        var result = await model.OnPostAsync(8);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ModelState[string.Empty]!.Errors.Should().ContainSingle();
    }
}