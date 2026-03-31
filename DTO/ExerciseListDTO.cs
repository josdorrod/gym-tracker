using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO
{
    public class ExerciseListDTO
    {
            /// <summary>Gets or sets the primary key.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the exercise.</summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the muscle group targeted by this exercise.</summary>
        [MaxLength(50)]
        public string? MuscleGroup { get; set; }    
    }
}