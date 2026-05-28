using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTracker.Data.Entities
{
    public class WorkoutSession
    {
        /// <summary>The unique identifier for the workout session.</summary>
        public int Id { get; set;}

        /// <summary>The foreign key to the associated plan.</summary>
        public int? PlanId { get; set;}

        /// <summary>The foreign key to the associated location.</summary>
        public int LocationId { get; set; }

        /// <summary>The date and time when the workout session took place.</summary>
        public DateTime StartTime { get; set; }

        /// <summary>The date and time when the workout session ended.</summary>
        public DateTime? EndTime { get; set; }


        /// <summary>The navigation property to the associated plan.</summary>
        public Plan? Plan { get; set; }

        /// <summary>The navigation property to the associated location.</summary>
        public Location Location { get; set; } = null!;

        /// <summary>The sets performed during the workout session.</summary>
        public ICollection<Set> Sets { get; set; } = new List<Set>();
    }
}