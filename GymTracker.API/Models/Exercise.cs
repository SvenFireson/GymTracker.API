namespace GymTracker.API.Models;

public class Exercise
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MuscleGroup { get; set; } = string.Empty;

    public string Equipment { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public bool IsCompound { get; set; }

    public string VideoUrl { get; set; } = string.Empty;
}