using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        return await _context.Todos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetTodoById(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();
        return todo;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo todo)
    {
        if (id != todo.Id) return BadRequest();

        _context.Entry(todo).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Todos.Any(e => e.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
