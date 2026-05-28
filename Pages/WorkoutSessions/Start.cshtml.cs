using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GymTracker.Pages.WorkoutSessions;

public class StartModel : PageModel
{
    private readonly ILocationService _locationService;
    private readonly IPlanService _planService;
    private readonly IWorkoutSessionService _sessionService;

    public StartModel(
        ILocationService locationService,
        IPlanService planService,
        IWorkoutSessionService sessionService)
    {
        _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        _planService = planService ?? throw new ArgumentNullException(nameof(planService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    [BindProperty]
    public WorkoutSessionStartDTO Input { get; set; } = new();

    public List<LocationListDTO> Locations { get; set; } = [];

    public List<PlanListDTO> Plans { get; set; } = [];

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        Locations = await _locationService.GetAllLocationsAsync();
        Plans = await _planService.GetAllPlansAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Locations = await _locationService.GetAllLocationsAsync();
            Plans = await _planService.GetAllPlansAsync();
            return Page();
        }

        var sessionDto = new WorkoutSessionCreateDTO
        {
            LocationId = Input.LocationId,
            PlanId = Input.PlanId
        };

        var sessionId = await _sessionService.CreateSessionAsync(sessionDto);

        if (!string.IsNullOrEmpty(ReturnUrl))
        {
            return Redirect(ReturnUrl);
        }

        return RedirectToPage("/Exercises/Index");
    }
}

public class WorkoutSessionStartDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una ubicación.")]
    public int LocationId { get; set; }

    public int? PlanId { get; set; }
}
