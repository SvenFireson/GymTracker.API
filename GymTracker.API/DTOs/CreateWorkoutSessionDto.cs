using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class CreateWorkoutSessionDto
{
    [Required]
    public int WorkoutId { get; set; }

    public string Notes { get; set; } = string.Empty;
}