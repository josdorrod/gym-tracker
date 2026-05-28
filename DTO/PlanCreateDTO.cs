using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO
{
    public class PlanCreateDTO
    {
        /// <summary>The name of the plan.</summary>
        [Required, MaxLength(100, ErrorMessage = "The name of the plan cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>The description of the plan.</summary>
        [MaxLength(200, ErrorMessage = "The description of the plan cannot exceed 200 characters.")]
        public string? Description { get; set; }

        /// <summary>Indicates whether the plan is active.</summary>
        public bool IsActive { get; set; } = true;
    }
}