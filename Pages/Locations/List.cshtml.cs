using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Locations;

/// <summary>
/// Renders the locations list fragment used by the modal workflow.
/// </summary>
public class ListModel(ILocationService locationService) : PageModel
{
    private readonly ILocationService _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

    /// <summary>
    /// Gets the available locations.
    /// </summary>
    public IList<LocationListDTO> Locations { get; private set; } = [];

    /// <summary>
    /// Loads the locations list fragment.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnGetAsync()
    {
        Locations = await _locationService.GetAllLocationsAsync();
    }
}