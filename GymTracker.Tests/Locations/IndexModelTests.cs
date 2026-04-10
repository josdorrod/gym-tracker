using FluentAssertions;
using GymTracker.DTO;
using GymTracker.Pages.Locations;
using GymTracker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GymTracker.Tests.Locations;

[TestClass]
public class IndexModelTests
{
    [TestMethod]
    public async Task OnGetAsync_AsignaLasUbicacionesRetornadasPorElServicio()
    {
        // Arrange
        var serviceMock = new Mock<ILocationService>();
        serviceMock.Setup(service => service.GetAllLocationsAsync()).ReturnsAsync(new List<LocationListDTO>
        {
            new() { Id = 1, Name = "Atlas Gym" },
            new() { Id = 2, Name = "Downtown Fit" }
        });

        var model = new GymTracker.Pages.Locations.IndexModel(serviceMock.Object);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Locations.Should().HaveCount(2);
        model.Locations.Select(location => location.Name).Should().ContainInOrder("Atlas Gym", "Downtown Fit");
    }
}