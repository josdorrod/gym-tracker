using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTracker.Data.Entities
{
    public class PlanExercise
    {
        public int PlanId { get; set; }
        public int ExerciseId  {get; set; }
        public int Order {get; set; }
    }
}