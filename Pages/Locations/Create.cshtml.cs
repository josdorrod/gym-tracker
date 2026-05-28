using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Locations;

/// <summary>
/// Handles location creation inside the modal dialog.
/// </summary>
public class CreateModel(ILocationService locationService) : PageModel
{
    private readonly ILocationService _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    [BindProperty]
    public LocationEditDTO Input { get; set; } = new();

    /// <summary>
    /// Prepares the create form.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <returns>The modal response.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _locationService.CreateLocationAsync(Input);
        return new NoContentResult();
    }
}