using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator,Lecturer")]
public class GroupController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GroupController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] Group group)
    {
        if (group == null)
        {
            return BadRequest("Invalid group data.");
        }

        var course = await _context.Courses.FindAsync(group.CourseId);
        if (course == null)
        {
            return NotFound($"Course with ID {group.CourseId} not found.");
        }

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(int id)
    {
        var group = await _context.Groups.Include(g => g.Course)
                                         .Include(g => g.Students)
                                         .FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return NotFound($"Group with ID {id} not found.");
        }

        return Ok(group);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] Group updatedGroup)
    {
        if (updatedGroup == null || updatedGroup.Id != id)
        {
            return BadRequest("Invalid group data.");
        }

        var existingGroup = await _context.Groups.Include(g => g.Course)
                                                 .FirstOrDefaultAsync(g => g.Id == id);
        if (existingGroup == null)
        {
            return NotFound($"Group with ID {id} not found.");
        }

        existingGroup.Name = updatedGroup.Name;
        existingGroup.CourseId = updatedGroup.CourseId;

        await _context.SaveChangesAsync();

        return Ok(existingGroup);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound($"Group with ID {id} not found.");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
