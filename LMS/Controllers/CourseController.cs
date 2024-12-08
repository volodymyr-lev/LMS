using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS.Data;
using System.Threading.Tasks;
using LMS.DTOs;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;

namespace LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Administrator")]
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

        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
            Credits = request.Credits,
            LecturerId = request.LecturerId,
            GroupCourses = request.GroupIds.Select(groupId => new GroupCourse
            {
                GroupId = groupId
            }).ToList()
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }


    [HttpGet("{id}")]
    [Authorize(Roles="Student")]
    public async Task<IActionResult> GetCourse(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Lecturer)
            .Include(c => c.GroupCourses)
                .ThenInclude(gc => gc.Group)
        .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        return Ok(course);
    }

    [HttpGet("by-group")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetCoursesByUserGroup()
    {
        var userId = _userManager.GetUserId(User);
        
        if (userId == null)
        {
            return Unauthorized("User is not logged in.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.GroupId == null)
        {
            return NotFound("User does not belong to any group.");
        }

        var courses = await _context.GroupCourses
            .Where(gc => gc.GroupId == user.GroupId)
            .Select(gc => gc.Course)
            .ToListAsync();

        return Ok(courses);
    }


    [HttpPut("update/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
    {
        if (request == null || id <= 0)
        {
            return BadRequest("Invalid request.");
        }

        var course = await _context.Courses
            .Include(c => c.GroupCourses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // Update course properties
        course.Name = request.Name;
        course.Description = request.Description;
        course.Credits = request.Credits;
        course.LecturerId = request.LecturerId;

        // Update group associations
        course.GroupCourses.Clear();
        course.GroupCourses = request.GroupIds.Select(groupId => new GroupCourse
        {
            CourseId = id,
            GroupId = groupId
        }).ToList();

        await _context.SaveChangesAsync();

        return Ok(course);
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Administrator")]
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
