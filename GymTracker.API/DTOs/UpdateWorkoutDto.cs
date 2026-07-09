using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs.Workouts
{
    public class UpdateWorkoutDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}