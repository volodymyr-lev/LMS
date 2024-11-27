using LMS.Data;
using LMS.Models;
using LMS.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Lecturer")]
public class FileVerificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FileVerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyFile([FromBody] FileVerificationRequest request)
    {
        var thesis = await _context.Theses
            .Include(t => t.Rules)
            .FirstOrDefaultAsync(t => t.FilePath == request.FilePath);

        if (thesis == null)
            return NotFound("File not found in the system.");

        var violations = new List<Violation>();

        // Перевірка правил
        foreach (var rule in thesis.Rules)
        {
            var ruleParams = await _context.RuleParameters
                .Where(p => p.RuleId == rule.Id)
                .ToListAsync();

            var ruleViolations = VerifyFileByRule(thesis.FilePath, rule, ruleParams);
            violations.AddRange(ruleViolations);
        }

        var thesisVerification = new ThesisVerification
        {
            ThesisId = thesis.Id,
            VerificationDate = DateTime.UtcNow,
            Result = violations.Any() ? "Failed" : "Passed",
            Comments = violations.Any() ? "Some rules were violated" : "All rules passed",
            Violations = violations
        };

        _context.ThesisVerifications.Add(thesisVerification);
        await _context.SaveChangesAsync();

        var verificationResult = new FileVerificationResultDto
        {
            Id = thesisVerification.Id,
            FileUploadId = thesis.Id,
            IsVerified = !violations.Any(),
            Violations = violations.Select(v => v.Description).ToList(),
            VerificationDate = thesisVerification.VerificationDate
        };

        return Ok(verificationResult);
    }

    private List<Violation> VerifyFileByRule(string filePath, Rule rule, List<RuleParameter> ruleParams)
    {
        var violations = new List<Violation>();

        // TODO: Реалізувати перевірку за допомогою OpenXML
        
        if (rule.Name == "UML Diagram Check")
        {
            var umlParam = ruleParams.FirstOrDefault(p => p.Name == "Mandatory");
            if (umlParam?.Value == "true" && !HasUMLDiagram(filePath))
            {
                violations.Add(new Violation
                {
                    Description = "Missing UML diagram"
                });
            }
        }

        
        return violations;
    }

    private bool HasUMLDiagram(string filePath)
    {
        return false;
    }
}

public class FileVerificationRequest
{
    public string FilePath { get; set; }
}