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
    public async Task<ActionResult> GetExercises(
     int page = 1,
     int pageSize = 10,
     string? sortBy = "name")
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 50)
            pageSize = 50;

        var query = _context.Exercises.AsQueryable();

        query = sortBy?.ToLower() switch
        {
            "muscle" => query.OrderBy(e => e.MuscleGroup),
            "difficulty" => query.OrderBy(e => e.Difficulty),
            "equipment" => query.OrderBy(e => e.Equipment),
            _ => query.OrderBy(e => e.Name)
        };

        var totalExercises = await query.CountAsync();

        var exercises = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            TotalExercises = totalExercises,
            TotalPages = (int)Math.Ceiling(totalExercises / (double)pageSize),
            Data = exercises
        });
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

        return Ok(workout);
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