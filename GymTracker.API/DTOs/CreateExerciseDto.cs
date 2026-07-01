using System.ComponentModel.DataAnnotations;

namespace GymTracker.API.DTOs;

public class CreateExerciseDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string MuscleGroup { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Equipment { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Difficulty { get; set; } = string.Empty;

    public bool IsCompound { get; set; }

    [Url]
    public string VideoUrl { get; set; } = string.Empty;
}