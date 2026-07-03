namespace GymTracker.API.DTOs;

public class WorkoutResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<string> Exercises { get; set; } = new();
}