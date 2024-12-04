using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using System.Threading.Tasks;
using LMS.DTOs;
using Microsoft.AspNetCore.Identity;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator,Lecturer")]
public class GroupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid group data.");
        }

        var group = new Group
        {
            Name = request.Name,
            Students = (ICollection<ApplicationUser>)await _userManager.Users
                .Where(u => request.StudentIds.Contains(u.Id))
                .ToListAsync()
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(int id)
    {
        var group = await _context.Groups
            .Include(g => g.GroupCourses)
                .ThenInclude(gc => gc.Course)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound($"Group with ID {id} not found.");
        }

        return Ok(group);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid group data.");
        }

        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound($"Group with ID {id} not found.");
        }

        group.Name = request.Name;
        group.Students = (ICollection<ApplicationUser>)await _userManager.Users
            .Where(u => request.StudentIds.Contains(u.Id))
            .ToListAsync();

        await _context.SaveChangesAsync();

        return Ok(group);
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
