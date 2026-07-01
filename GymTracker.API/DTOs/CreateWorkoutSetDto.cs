using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class CreateWorkoutSetDto
{
    [Required]
    public int WorkoutSessionId { get; set; }

    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public int SetNumber { get; set; }

    [Required]
    public decimal Weight { get; set; }

    [Required]
    public int Reps { get; set; }

    public string Notes { get; set; } = string.Empty;
}