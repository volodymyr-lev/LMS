using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LMS.Controllers;
[ApiController]
[Route("api/file")]
[Authorize(Roles = "Student")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly FileUploadOptions _fileUploadOptions;

    public FileUploadController(IWebHostEnvironment env, IOptions<FileUploadOptions> fileUploadOptions)
    {
        _env = env;
        _fileUploadOptions = fileUploadOptions.Value;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var extension = Path.GetExtension(file.FileName);
        if (!_fileUploadOptions.AllowedExtensions.Contains(extension.ToLower()))
            return BadRequest("Unsupported file format.");

        if (file.Length > _fileUploadOptions.MaxFileSize)
            return BadRequest("File is too large.");

        var uploadsPath = Path.Combine(_env.WebRootPath, _fileUploadOptions.Directory);
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, Guid.NewGuid() + extension);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { FilePath = filePath });
    }
}