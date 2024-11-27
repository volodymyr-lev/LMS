using LMS.Models;
using LMS.DTOs;
using LMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers;

[ApiController]
[Route("api/coursework/file")]
[Authorize(Roles = "Student")]
public class CourseWorkFileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly FileUploadOptions _fileUploadOptions;
    private readonly ApplicationDbContext _context;

    public CourseWorkFileUploadController(
        IWebHostEnvironment env,
        IOptions<FileUploadOptions> fileUploadOptions,
        ApplicationDbContext context)
    {
        _env = env;
        _fileUploadOptions = fileUploadOptions.Value;
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCourseWorkFile(IFormFile file,
        [FromQuery] string title,
        [FromQuery] string description,
        [FromQuery] string advisorId,
        [FromQuery] string studentId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var extension = Path.GetExtension(file.FileName);
        if (!_fileUploadOptions.AllowedExtensions.Contains(extension.ToLower()))
            return BadRequest("Unsupported file format.");

        if (file.Length > _fileUploadOptions.MaxFileSize)
            return BadRequest("File is too large.");

        var student = await _context.Users.FindAsync(studentId);
        if (student == null)
            return BadRequest("Specified student not found.");

        // Перевірка, чи існує такий керівник
        var advisor = await _context.Users.FindAsync(advisorId);
        if (advisor == null)
            return BadRequest("Specified advisor not found.");

        var uploadsPath = Path.Combine(_fileUploadOptions.Directory, "CourseWork");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"CourseWork_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var courseWork = new CourseWork
        {
            Title = title,
            Description = description,
            FilePath = filePath,
            StudentId = studentId,
            AdvisorId = advisorId,
            Status = "submitted",
            Score = null
        };

        _context.CourseWorks.Add(courseWork);
        await _context.SaveChangesAsync();

        var fileUploadDto = new FileUploadDTO
        {
            Id = courseWork.Id,
            FileName = file.FileName,
            FilePath = filePath,
            UploadDate = DateTime.UtcNow,
            UserId = courseWork.StudentId,
            MentorOrAdvisorId = courseWork.AdvisorId,
            FileType = extension,
            FileSize = file.Length
        };

        return Ok(fileUploadDto);
    }

    [HttpPut("update/{courseWorkId}")]
    public async Task<IActionResult> UpdateCourseWorkFile(int courseWorkId, IFormFile file)
    {
        var courseWork = await _context.CourseWorks.FindAsync(courseWorkId);
        if (courseWork == null)
            return NotFound("Course work not found.");

        if (courseWork.StudentId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return Forbid("You can only update your own course work.");

        var extension = Path.GetExtension(file.FileName);
        var uploadsPath = Path.Combine(_env.WebRootPath, _fileUploadOptions.Directory, "CourseWork");
        var fileName = $"CourseWork_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Видаляємо попередній файл, якщо він існує
        if (!string.IsNullOrEmpty(courseWork.FilePath) && System.IO.File.Exists(courseWork.FilePath))
        {
            System.IO.File.Delete(courseWork.FilePath);
        }

        courseWork.FilePath = filePath;
        courseWork.Status = "resubmitted";

        _context.CourseWorks.Update(courseWork);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Course work file updated successfully" });
    }
}