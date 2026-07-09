using GymTracker.API.Data;
using GymTracker.API.DTOs;
using GymTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkoutsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkoutResponseDto>>> GetWorkouts()
    {
        var workouts = await _context.Workouts
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .ToListAsync();

        var response = workouts.Select(w => new WorkoutResponseDto
        {
            Id = w.Id,
            Name = w.Name,
            CreatedAt = w.CreatedAt,
            Exercises = w.WorkoutExercises
                .Select(we => we.Exercise.Name)
                .ToList()
        }).ToList();

        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutResponseDto>> GetWorkoutById(int id)
    {
        var workout = await _context.Workouts
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
            return NotFound();

        var response = new WorkoutResponseDto
        {
            Id = workout.Id,
            Name = workout.Name,
            CreatedAt = workout.CreatedAt,
            Exercises = workout.WorkoutExercises
                .Select(we => we.Exercise.Name)
                .ToList()
        };

        return Ok(response);
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

        var response = new WorkoutResponseDto
        {
            Id = workout.Id,
            Name = workout.Name,
            CreatedAt = workout.CreatedAt,
            Exercises = new List<string>()
        };

        return CreatedAtAction(nameof(GetWorkoutById), new { id = workout.Id }, response);
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
        var alreadyExists = await _context.WorkoutExercises
    .AnyAsync(we => we.WorkoutId == workoutId && we.ExerciseId == dto.ExerciseId);

        if (alreadyExists)
            return BadRequest("This exercise is already in the workout.");
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