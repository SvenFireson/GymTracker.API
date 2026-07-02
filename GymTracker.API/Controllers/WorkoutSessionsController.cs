using GymTracker.API.Data;
using GymTracker.API.DTOs;
using GymTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkoutSessionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkoutSessionResponseDto>>> GetWorkoutSessions()
    {
        var sessions = await _context.WorkoutSessions
            .Include(ws => ws.Workout)
            .ToListAsync();

        var response = sessions.Select(ws => new WorkoutSessionResponseDto
        {
            Id = ws.Id,
            WorkoutId = ws.WorkoutId,
            WorkoutName = ws.Workout.Name,
            StartedAt = ws.StartedAt,
            CompletedAt = ws.CompletedAt,
            Notes = ws.Notes
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutSessionResponseDto>> GetWorkoutSession(int id)
    {
        var session = await _context.WorkoutSessions
            .Include(ws => ws.Workout)
            .FirstOrDefaultAsync(ws => ws.Id == id);

        if (session == null)
            return NotFound();

        var response = new WorkoutSessionResponseDto
        {
            Id = session.Id,
            WorkoutId = session.WorkoutId,
            WorkoutName = session.Workout.Name,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            Notes = session.Notes
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSession>> CreateWorkoutSession(CreateWorkoutSessionDto dto)
    {
        var workout = await _context.Workouts.FindAsync(dto.WorkoutId);

        if (workout == null)
            return NotFound("Workout not found.");

        var session = new WorkoutSession
        {
            WorkoutId = dto.WorkoutId,
            Notes = dto.Notes
        };

        _context.WorkoutSessions.Add(session);
        await _context.SaveChangesAsync();

        var response = new WorkoutSessionResponseDto
        {
            Id = session.Id,
            WorkoutId = session.WorkoutId,
            WorkoutName = workout.Name,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            Notes = session.Notes
        };

        return CreatedAtAction(nameof(GetWorkoutSession), new { id = session.Id }, response);
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteWorkoutSession(int id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);

        if (session == null)
            return NotFound();

        session.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutSession(int id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);

        if (session == null)
            return NotFound();

        _context.WorkoutSessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}