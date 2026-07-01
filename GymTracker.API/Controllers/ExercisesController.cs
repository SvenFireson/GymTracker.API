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
    public async Task<ActionResult<List<Exercise>>> GetExercises()
    {
        return await _context.Exercises.ToListAsync();
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
}