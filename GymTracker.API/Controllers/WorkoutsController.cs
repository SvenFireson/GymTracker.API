using GymTracker.API.Data;
using GymTracker.API.DTOs;
using GymTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkoutsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Workout>>> GetWorkouts()
    {
        return await _context.Workouts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Workout>> GetWorkout(int id)
    {
        var workout = await _context.Workouts.FindAsync(id);

        if (workout == null)
            return NotFound();

        return workout;
    }

    [HttpPost]
    public async Task<ActionResult<Workout>> CreateWorkout(CreateWorkoutDto dto)
    {
        var workout = new Workout
        {
            Name = dto.Name
        };

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkout), new { id = workout.Id }, workout);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkout(int id, UpdateWorkoutDto dto)
    {
        var workout = await _context.Workouts.FindAsync(id);

        if (workout == null)
            return NotFound();

        workout.Name = dto.Name;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkout(int id)
    {
        var workout = await _context.Workouts.FindAsync(id);

        if (workout == null)
            return NotFound();

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPost("{workoutId}/exercises")]
    public async Task<IActionResult> AddExerciseToWorkout(int workoutId, AddExerciseToWorkoutDto dto)
    {
        var workout = await _context.Workouts.FindAsync(workoutId);

        if (workout == null)
            return NotFound("Workout not found.");

        var exercise = await _context.Exercises.FindAsync(dto.ExerciseId);

        if (exercise == null)
            return NotFound("Exercise not found.");

        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = workoutId,
            ExerciseId = dto.ExerciseId
        };

        _context.WorkoutExercises.Add(workoutExercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}