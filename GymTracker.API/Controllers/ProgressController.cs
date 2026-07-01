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
}