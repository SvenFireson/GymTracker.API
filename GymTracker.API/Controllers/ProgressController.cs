using GymTracker.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProgressController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("personal-record/{exerciseId}")]
    public async Task<IActionResult> GetPersonalRecord(int exerciseId)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseId);

        if (exercise == null)
            return NotFound("Exercise not found.");

        var bestSet = await _context.WorkoutSets
            .Where(ws => ws.ExerciseId == exerciseId)
            .OrderByDescending(ws => ws.Weight)
            .ThenByDescending(ws => ws.Reps)
            .FirstOrDefaultAsync();

        if (bestSet == null)
            return NotFound("No sets found for this exercise.");

        return Ok(new
        {
            Exercise = exercise.Name,
            MaxWeight = bestSet.Weight,
            Reps = bestSet.Reps,
            Notes = bestSet.Notes
        });
    }
    [HttpGet("history/{exerciseId}")]
    public async Task<IActionResult> GetExerciseHistory(int exerciseId)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseId);

        if (exercise == null)
            return NotFound("Exercise not found.");

        var history = await _context.WorkoutSets
            .Where(ws => ws.ExerciseId == exerciseId)
            .Include(ws => ws.WorkoutSession)
            .OrderBy(ws => ws.WorkoutSession.StartedAt)
            .Select(ws => new
            {
                Date = ws.WorkoutSession.StartedAt,
                Exercise = exercise.Name,
                ws.SetNumber,
                ws.Weight,
                ws.Reps,
                ws.Notes
            })
            .ToListAsync();

        return Ok(history);
    }
    [HttpGet("volume/{exerciseId}")]
    public async Task<IActionResult> GetExerciseVolume(int exerciseId)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseId);

        if (exercise == null)
            return NotFound("Exercise not found.");

        var sets = await _context.WorkoutSets
            .Where(ws => ws.ExerciseId == exerciseId)
            .ToListAsync();

        if (!sets.Any())
            return NotFound("No sets found for this exercise.");

        var totalVolume = sets.Sum(ws => ws.Weight * ws.Reps);
        var totalSets = sets.Count;
        var totalReps = sets.Sum(ws => ws.Reps);

        return Ok(new
        {
            Exercise = exercise.Name,
            TotalVolume = totalVolume,
            TotalSets = totalSets,
            TotalReps = totalReps
        });
    }
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var recentSessions = await _context.WorkoutSessions
            .Where(ws => ws.StartedAt >= sevenDaysAgo)
            .ToListAsync();

        var recentSets = await _context.WorkoutSets
            .Include(ws => ws.WorkoutSession)
            .Where(ws => ws.WorkoutSession.StartedAt >= sevenDaysAgo)
            .ToListAsync();

        var totalVolume = recentSets.Sum(ws => ws.Weight * ws.Reps);

        return Ok(new
        {
            SessionsThisWeek = recentSessions.Count,
            SetsCompleted = recentSets.Count,
            TotalVolume = totalVolume,
            TotalReps = recentSets.Sum(ws => ws.Reps)
        });

    }

    [HttpGet("workout-summary/{sessionId}")]
    public async Task<IActionResult> GetWorkoutSummary(int sessionId)
    {
        var session = await _context.WorkoutSessions
            .Include(ws => ws.WorkoutSets)
                .ThenInclude(s => s.Exercise)
            .FirstOrDefaultAsync(ws => ws.Id == sessionId);

        if (session == null)
            return NotFound("Workout session not found.");

        return Ok(new
        {
            SessionId = session.Id,
            Started = session.StartedAt,
            Completed = session.CompletedAt,
            TotalExercises = session.WorkoutSets
                .Select(s => s.ExerciseId)
                .Distinct()
                .Count(),
            TotalSets = session.WorkoutSets.Count,
            TotalVolume = session.WorkoutSets.Sum(s => s.Weight * s.Reps),
            Exercises = session.WorkoutSets.Select(s => new
            {
                Exercise = s.Exercise.Name,
                s.SetNumber,
                s.Weight,
                s.Reps
            })
        });
    }


}