namespace GymTracker.API.Models;

public class WorkoutSession
{
    public int Id { get; set; }

    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public string Notes { get; set; } = string.Empty;
    public List<WorkoutSet> WorkoutSets { get; set; } = new();
}