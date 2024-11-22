using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS.Data;
using System.Threading.Tasks;
using LMS.DTOs;

namespace LMS.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class CourseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CourseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid course data.");
        }

        var lecturer = await _userManager.FindByIdAsync(request.LecturerId);
        if (lecturer == null)
        {
            return BadRequest("Lecturer not found.");
        }

        var groups = await _context.Groups.Where(g => request.GroupIds.Contains(g.Id)).ToListAsync();
        if (groups.Count != request.GroupIds.Count)
        {
            return BadRequest("Some groups were not found.");
        }

        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
            Credits = request.Credits,
            LecturerId = request.LecturerId,
            Groups = groups
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(int id)
    {
        var course = await _context.Courses.Include(c => c.Lecturer)
                                           .Include(c => c.Groups)
                                           .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        return Ok(course);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
    {
        if (request == null || id <= 0)
        {
            return BadRequest("Invalid request.");
        }

        var course = await _context.Courses.Include(c => c.Groups).FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var groups = await _context.Groups.Where(g => request.GroupIds.Contains(g.Id)).ToListAsync();
        if (groups.Count != request.GroupIds.Count)
        {
            return BadRequest("Some groups were not found.");
        }

        course.Name = request.Name;
        course.Description = request.Description;
        course.Credits = request.Credits;
        course.LecturerId = request.LecturerId;
        course.Groups = groups;

        await _context.SaveChangesAsync();

        return Ok(course);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
