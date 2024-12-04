using LMS.Data;
using LMS.Models;
using LMS.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using UglyToad.PdfPig;

namespace LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Lecturer")]
public class ThesisFileVerificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ThesisFileVerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyFile([FromBody] FileVerificationRequest request)
    {
        // Знаходимо тезу за ID
        var thesis = await _context.Theses
            .FirstOrDefaultAsync(t => t.Id == Convert.ToInt32(request.RuleId));

        if (thesis == null)
            return NotFound($"Thesis with ID {request.FileId} not found.");

        // Знаходимо правило та його параметри
        var rule = await _context.Rules
            .Include(r => r.RuleParameters)
            .FirstOrDefaultAsync(r => r.Id == Convert.ToInt32(request.RuleId));

        if (rule == null)
            return NotFound($"Rule with ID {request.RuleId} not found.");

        var ruleParams = await _context.RuleRuleParameters
            .Where(rp => rp.RuleId == Convert.ToInt32(request.RuleId))
            .Include(rp => rp.RuleParameter)
            .Select(rp => rp.RuleParameter) 
            .Distinct() 
            .ToListAsync();



        if (!ruleParams.Any())
            return NotFound($"No parameters found for rule ID {request.RuleId}.");

        // Відкриваємо файл і перевіряємо його вміст
        var violations = VerifyFileContentByParameters(thesis.FilePath, ruleParams);

        // Формуємо результат перевірки
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

        // Повертаємо DTO
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

    private List<Violation> VerifyFileContentByParameters(string filePath, List<RuleParameter> ruleParams)
    {
        var violations = new List<Violation>();

        // Check if file exists
        if (!System.IO.File.Exists(filePath))
        {
            violations.Add(new Violation { Description = "File does not exist." });
            return violations;
        }

        try
        {
            string content = "";
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".pdf")
            {
                content = ExtractTextFromPdf(filePath);
            }
            else if (extension == ".docx")
            {
                content = ExtractTextFromDocx(filePath);
            }
            else
            {
                violations.Add(new Violation
                {
                    Description = "Unsupported file format. Only .pdf and .docx are allowed."
                });
                return violations;
            }

            // Perform checks for each parameter
            foreach (var param in ruleParams)
            {
                var parameterName = param.Name;
                var parameterValue = param.Value;

                switch (parameterName)
                {
                    case "Mandatory":
                        if (parameterValue == "true")
                        {
                            violations.Add(new Violation
                            {
                                Description = "Missing UML diagram in the file content."
                            });
                        }
                        break;

                    case "MaxPages":
                        if (int.TryParse(parameterValue, out int maxPages))
                        {
                            int pageCount = extension == ".pdf" ? CountPdfPages(filePath) : CountDocxPages(content);
                            if (pageCount > maxPages)
                            {
                                violations.Add(new Violation
                                {
                                    Description = $"File exceeds the maximum page limit of {maxPages}. Current pages: {pageCount}."
                                });
                            }
                        }
                        break;

                    case "BanWord":
                        if (content.Contains(parameterValue, StringComparison.OrdinalIgnoreCase))
                        {
                            violations.Add(new Violation
                            {
                                Description = $"The file contains a banned word: {parameterValue}."
                            });
                        }
                        break;

                    default:
                        violations.Add(new Violation
                        {
                            Description = $"Unknown rule parameter: {parameterName}."
                        });
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            violations.Add(new Violation
            {
                Description = $"Error while reading the file: {ex.Message}"
            });
        }

        return violations;
    }

    private string ExtractTextFromPdf(string filePath)
    {
        var sb = new StringBuilder();
        using (var pdfReader = new PdfReader(filePath))
        using (var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader))
        {
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                sb.AppendLine($"Page {i}: {text}");
            }
        }
        return sb.ToString();
    }


    // Extract text from a .docx file using OpenXML
    private string ExtractTextFromDocx(string filePath)
    {
        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
        {
            var body = doc.MainDocumentPart.Document.Body;
            return body.InnerText;
        }
    }

    // Count pages in a .docx file (as an approximation based on page breaks)
    private int CountDocxPages(string content)
    {
        return content.Split(new[] { "\f" }, StringSplitOptions.None).Length;
    }

    // Count pages in a PDF file using PDFSharp
    private int CountPdfPages(string filePath)
    {
        using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(filePath))
        {
            return pdfDocument.NumberOfPages;
        }
    }
}