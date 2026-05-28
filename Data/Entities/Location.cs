using System.ComponentModel.DataAnnotations;

namespace GymTracker.Data.Entities
{
    public class Location
    {
        /// <summary>The unique identifier for the location.</summary>
        public int Id { get; set;}

        /// <summary>The name of the location.</summary>
        [Required]
        public string Name { get; set;} = null!;
    }
}