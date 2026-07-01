using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class UpdateWorkoutDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}