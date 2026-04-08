using System.ComponentModel.DataAnnotations;

namespace GymTracker.Data.Entities
{
    public class Plan
    {
        /// <summary>The unique identifier for the plan.</summary>
        public int Id { get; set;}

        /// <summary>The name of the plan.</summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>The description of the plan.</summary>
        public string? Description { get; set; }
    
        /// <summary>The exercises included in the plan.</summary>
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}