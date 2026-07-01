using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class CreateWorkoutDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}