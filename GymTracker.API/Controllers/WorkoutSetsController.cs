using GymTracker.API.Data;
using GymTracker.API.DTOs;
using GymTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkoutSetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkoutSet>>> GetWorkoutSets()
    {
        return await _context.WorkoutSets
            .Include(ws => ws.Exercise)
            .Include(ws => ws.WorkoutSession)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSet>> CreateWorkoutSet(CreateWorkoutSetDto dto)
    {
        var session = await _context.WorkoutSessions.FindAsync(dto.WorkoutSessionId);
        if (session == null)
            return NotFound("Workout session not found.");

        var exercise = await _context.Exercises.FindAsync(dto.ExerciseId);
        if (exercise == null)
            return NotFound("Exercise not found.");

        var workoutSet = new WorkoutSet
        {
            WorkoutSessionId = dto.WorkoutSessionId,
            ExerciseId = dto.ExerciseId,
            SetNumber = dto.SetNumber,
            Weight = dto.Weight,
            Reps = dto.Reps,
            Notes = dto.Notes
        };

        _context.WorkoutSets.Add(workoutSet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkoutSets), new { id = workoutSet.Id }, workoutSet);
    }
}