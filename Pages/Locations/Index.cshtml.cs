using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Locations;

/// <summary>
/// Displays the locations management page.
/// </summary>
public class IndexModel(ILocationService locationService) : PageModel
{
    private readonly ILocationService _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

    /// <summary>
    /// Gets the available locations.
    /// </summary>
    public IList<LocationListDTO> Locations { get; private set; } = [];

    /// <summary>
    /// Loads the locations list.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnGetAsync()
    {
        Locations = await _locationService.GetAllLocationsAsync();
    }
}