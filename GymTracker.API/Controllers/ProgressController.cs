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
}