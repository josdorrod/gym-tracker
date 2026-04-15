using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GymTracker.Data.Entities
{
    public class PlanExercise
    {
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
        public int ExerciseId  {get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int Order {get; set; }
    }
}