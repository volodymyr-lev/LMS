﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS.Data;
using System.Threading.Tasks;
using LMS.DTOs;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;
using DocumentFormat.OpenXml.Office2010.Excel;

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
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetCourse(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Lecturer)
            .Include(c => c.GroupCourses)
                .ThenInclude(gc => gc.Group)
            .Include(c => c.Assignments)
                .ThenInclude(cw => cw.CourseWorks)
            .Include(c => c.Assignments)
                .ThenInclude(t => t.Theses)             // dunno mb assignment for student doesn't need theses and cw lol
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
            .Include(gc => gc.Course) 
                .ThenInclude(c => c.Lecturer) 
            .Select(gc => gc.Course) 
            .ToListAsync();


        return Ok(courses);
    }

    [HttpGet("get-lecturer-courses")]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> GetLecturerCourses()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Unauthorized("User is not logged in.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        var courses = await _context.GroupCourses
            .Where(gc => gc.Course.LecturerId == user.Id)
            .Include(gc => gc.Course)
                .ThenInclude(c => c.Lecturer)
            .Select(gc => gc.Course)
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("get-course-submissions/{id}")]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> GetCourseSubmissions(int id)
    {
        var submissions = await _context.Assignments
            .Where(a => a.CourseId == id)
            .Include(a => a.CourseWorks)
                .ThenInclude(cw => cw.Student)
            .Include(a => a.Theses)
                .ThenInclude(t => t.Student)
            .ToListAsync();

        var studentSubmissions = new List<StudentSubmission>();

        foreach (var assignment in submissions)
        {
            studentSubmissions.AddRange(assignment.CourseWorks
                .Select(cw => new StudentSubmission
                {
                    studentId = cw.StudentId,
                    userName = cw.Student.UserName,
                    assignmentId = assignment.Id.ToString(),
                    assignmentTitle = assignment.Title,
                    status = cw.FilePath != null ? "Submitted" : "Not Submitted"
                }));

            studentSubmissions.AddRange(assignment.Theses
                .Select(t => new StudentSubmission
                {
                    studentId = t.StudentId,
                    userName = t.Student.UserName,
                    assignmentId = assignment.Id.ToString(),
                    assignmentTitle = assignment.Title,
                    status = t.FilePath != null ? "Submitted" : "Not Submitted"
                }));
        }

        return Ok(studentSubmissions);
    }

    [HttpGet("get-file-id")]
    [Authorize(Roles = "Student,Lecturer")]
    public async Task<IActionResult> GetFileId(int courseId, string studentId)
    {
        if (string.IsNullOrEmpty(studentId) || courseId <= 0)
        {
            return BadRequest("Invalid courseId or studentId.");
        }

        var file = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .SelectMany(a => a.CourseWorks)
            .FirstOrDefaultAsync(cw => cw.StudentId == studentId);

        if (file == null)
        {
            return NotFound("File not found for the given courseId and studentId.");
        }

        return Ok(new { FileId = file.Id });
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

internal class StudentSubmission
{
    public string studentId { get; set; }
    public string userName { get; set; }
    public string assignmentId { get; set; }
    public string assignmentTitle { get; set; }
    public DateTime submissionDate { get; set; }
    public string status { get; set; }
}