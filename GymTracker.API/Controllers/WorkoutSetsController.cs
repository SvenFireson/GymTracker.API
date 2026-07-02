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
    public async Task<ActionResult<List<WorkoutSetResponseDto>>> GetWorkoutSets()
    {
        var sets = await _context.WorkoutSets
            .Include(ws => ws.Exercise)
            .ToListAsync();

        var response = sets.Select(ws => new WorkoutSetResponseDto
        {
            Id = ws.Id,
            WorkoutSessionId = ws.WorkoutSessionId,
            ExerciseId = ws.ExerciseId,
            ExerciseName = ws.Exercise.Name,
            SetNumber = ws.SetNumber,
            Weight = ws.Weight,
            Reps = ws.Reps,
            Volume = ws.Weight * ws.Reps,
            Notes = ws.Notes
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSetResponseDto>> CreateWorkoutSet(CreateWorkoutSetDto dto)
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

        var response = new WorkoutSetResponseDto
        {
            Id = workoutSet.Id,
            WorkoutSessionId = workoutSet.WorkoutSessionId,
            ExerciseId = workoutSet.ExerciseId,
            ExerciseName = exercise.Name,
            SetNumber = workoutSet.SetNumber,
            Weight = workoutSet.Weight,
            Reps = workoutSet.Reps,
            Volume = workoutSet.Weight * workoutSet.Reps,
            Notes = workoutSet.Notes
        };

        return CreatedAtAction(nameof(GetWorkoutSets), new { id = workoutSet.Id }, response);
    }
}