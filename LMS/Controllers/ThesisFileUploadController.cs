using LMS.Models;
using LMS.DTOs;
using LMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers;

[ApiController]
[Route("api/thesis/file")]
[Authorize(Roles = "Student")]
public class ThesisFileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly FileUploadOptions _fileUploadOptions;
    private readonly ApplicationDbContext _context;

    public ThesisFileUploadController(
        IWebHostEnvironment env,
        IOptions<FileUploadOptions> fileUploadOptions,
        ApplicationDbContext context)
    {
        _env = env;
        _fileUploadOptions = fileUploadOptions.Value;
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadThesisFile(IFormFile file,
        [FromQuery] string title,
        [FromQuery] string description,
        [FromQuery] string mentorId,
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

        var mentor = await _context.Users.FindAsync(mentorId);
        if (mentor == null)
            return BadRequest("Specified mentor not found.");

        var uploadsPath = Path.Combine(_fileUploadOptions.Directory, "Thesis");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"Thesis_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var thesis = new Thesis
        {
            Title = title,
            Description = description,
            FilePath = filePath,
            StudentId = studentId,
            MentorId = mentorId,
            Status = "submitted",
            Score = null
        };

        _context.Theses.Add(thesis);
        await _context.SaveChangesAsync();

        var fileUploadDto = new FileUploadDTO
        {
            Id = thesis.Id,
            FileName = file.FileName,
            FilePath = filePath,
            UploadDate = DateTime.UtcNow,
            UserId = thesis.StudentId,
            MentorOrAdvisorId = thesis.MentorId,
            FileType = extension,
            FileSize = file.Length
        };

        return Ok(fileUploadDto);
    }

    [HttpPut("update/{thesisId}")]
    public async Task<IActionResult> UpdateThesisFile(int thesisId, IFormFile file)
    {
        var thesis = await _context.Theses.FindAsync(thesisId);
        if (thesis == null)
            return NotFound("Thesis not found.");

        if (thesis.StudentId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return Forbid("You can only update your own thesis.");

        var extension = Path.GetExtension(file.FileName);
        var uploadsPath = Path.Combine(_env.WebRootPath, _fileUploadOptions.Directory, "Thesis");
        var fileName = $"Thesis_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        if (!string.IsNullOrEmpty(thesis.FilePath) && System.IO.File.Exists(thesis.FilePath))
        {
            System.IO.File.Delete(thesis.FilePath);
        }

        thesis.FilePath = filePath;
        thesis.Status = "resubmitted";

        _context.Theses.Update(thesis);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Thesis file updated successfully" });
    }
}