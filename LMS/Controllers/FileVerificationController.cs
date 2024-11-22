using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles="Lecturer")]
public class FileVerificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FileVerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("verify")]
    public IActionResult VerifyFile([FromBody] FileVerificationRequest request)
    {
        var thesis = _context.Theses.FirstOrDefault(t => t.FilePath == request.FilePath);
        if (thesis == null)
            return NotFound("File not found in the system.");

        // TODO: OpenXML verification logic

        return Ok(new
        {
            Message = "File verification completed.",
            Violations = new[] { "Missing UML diagram.", "Incorrect font size." }
        });
    }
}

public class FileVerificationRequest
{
    public string FilePath { get; set; }
}
