namespace GymTracker.API.DTOs;

public class WorkoutSetResponseDto
{
    public int Id { get; set; }
    public int WorkoutSessionId { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public decimal Weight { get; set; }
    public int Reps { get; set; }
    public decimal Volume { get; set; }
    public string Notes { get; set; } = string.Empty;
}