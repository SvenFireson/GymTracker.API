using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs.Workouts
{
    public class CreateWorkoutDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

      
    }
}