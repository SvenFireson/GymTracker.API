using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class AddExerciseToWorkoutDto
{
    [Required]
    public int ExerciseId { get; set; }
}