using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTracker.DTO
{
    public class PlanListDTO
    {
        /// <summary>The unique identifier for the plan.</summary>
        public int Id { get; set;}

        /// <summary>The name of the plan.</summary>
        public string Name { get; set; } = null!;

        /// <summary>The description of the plan.</summary>
        public string? Description { get; set; }
    }
}