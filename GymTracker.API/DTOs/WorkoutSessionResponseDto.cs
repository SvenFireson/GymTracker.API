namespace GymTracker.API.DTOs;

public class WorkoutSessionResponseDto
{
    public int Id { get; set; }
    public int WorkoutId { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}