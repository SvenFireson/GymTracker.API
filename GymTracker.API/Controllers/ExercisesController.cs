using GymTracker.API.DTOs;
using GymTracker.API.Data;
using GymTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExercisesController(ApplicationDbContext context)
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
    .Select(e => new ExerciseResponseDto
    {
        Id = e.Id,
        Name = e.Name,
        MuscleGroup = e.MuscleGroup,
        Equipment = e.Equipment,
        Description = e.Description,
        Difficulty = e.Difficulty,
        IsCompound = e.IsCompound,
        VideoUrl = e.VideoUrl
    })
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetExercise(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);

        if (exercise == null)
            return NotFound();

        return exercise;
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> CreateExercise(CreateExerciseDto dto)
    {
        var exercise = new Exercise
        {
            Name = dto.Name,
            MuscleGroup = dto.MuscleGroup,
            Equipment = dto.Equipment,
            Description = dto.Description,
            Difficulty = dto.Difficulty,
            IsCompound = dto.IsCompound,
            VideoUrl = dto.VideoUrl
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(int id, UpdateExerciseDto dto)
    {
        var exercise = await _context.Exercises.FindAsync(id);

        if (exercise == null)
            return NotFound();

        exercise.Name = dto.Name;
        exercise.MuscleGroup = dto.MuscleGroup;
        exercise.Equipment = dto.Equipment;
        exercise.Description = dto.Description;
        exercise.Difficulty = dto.Difficulty;
        exercise.IsCompound = dto.IsCompound;
        exercise.VideoUrl = dto.VideoUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);

        if (exercise == null)
            return NotFound();

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("search")]
    public async Task<IActionResult> SearchExercises(
    string? muscleGroup,
    string? difficulty)
    {
        var query = _context.Exercises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            query = query.Where(e => e.MuscleGroup == muscleGroup);
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            query = query.Where(e => e.Difficulty == difficulty);
        }

        var exercises = await query.ToListAsync();

        return Ok(exercises);
    }
}