namespace GymTracker.API.Models;

public class Workout
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<WorkoutExercise> WorkoutExercises { get; set; } = new();
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}