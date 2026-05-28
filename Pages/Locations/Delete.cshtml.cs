using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Locations;

/// <summary>
/// Handles location deletion inside the modal dialog.
/// </summary>
public class DeleteModel(ILocationService locationService) : PageModel
{
    private readonly ILocationService _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

    /// <summary>
    /// Gets the location to display in the confirmation dialog.
    /// </summary>
    public LocationListDTO Location { get; private set; } = new();

    /// <summary>
    /// Loads the delete confirmation dialog.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The modal response.</returns>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        if (location is null)
        {
            return NotFound();
        }

        Location = location;
        return Page();
    }

    /// <summary>
    /// Deletes the requested location if it is not in use.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The modal response.</returns>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var deleteResult = await _locationService.DeleteLocationAsync(id);
        if (deleteResult == LocationDeleteResult.NotFound)
        {
            return NotFound();
        }

        if (deleteResult == LocationDeleteResult.InUse)
        {
            Location = await _locationService.GetLocationByIdAsync(id) ?? new LocationListDTO { Id = id };
            ModelState.AddModelError(string.Empty, "No se puede eliminar esta ubicación porque ya está asociada a sesiones de entrenamiento.");
            return Page();
        }

        return new NoContentResult();
    }
}