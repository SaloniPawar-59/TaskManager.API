using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    // Dependency Injection: ASP.NET Core provides the Database Context here
    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // 1. READ ALL (GET)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        return await _context.Tasks.ToListAsync();
    }

    // 2. READ ONE (GET by ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound(new { Message = $"Task {id} not found." });
        return Ok(task);
    }

    // 3. CREATE (POST)
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem newItem)
    {
        _context.Tasks.Add(newItem);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
    }

    // 4. UPDATE (PUT)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskItem updatedItem)
    {
        var existingTask = await _context.Tasks.FindAsync(id);
        if (existingTask == null) return NotFound();

        existingTask.Title = updatedItem.Title;
        existingTask.Description = updatedItem.Description;
        existingTask.IsCompleted = updatedItem.IsCompleted;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tasks.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // 5. DELETE (DELETE)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}