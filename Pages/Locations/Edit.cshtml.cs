using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Locations;

/// <summary>
/// Handles location edition inside the modal dialog.
/// </summary>
public class EditModel(ILocationService locationService) : PageModel
{
    private readonly ILocationService _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

    /// <summary>
    /// Gets or sets the editable location data.
    /// </summary>
    [BindProperty]
    public LocationEditDTO Input { get; set; } = new();

    /// <summary>
    /// Loads a location into the edit form.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The modal response.</returns>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var location = await _locationService.GetLocationForEditAsync(id);
        if (location is null)
        {
            return NotFound();
        }

        Input = location;
        return Page();
    }

    /// <summary>
    /// Updates a location.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The modal response.</returns>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updated = await _locationService.UpdateLocationAsync(id, Input);
        if (!updated)
        {
            return NotFound();
        }

        return new NoContentResult();
    }
}