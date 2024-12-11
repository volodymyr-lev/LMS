using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using LMS.DTOs;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;

namespace LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseWorkFileVerificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CourseWorkFileVerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyFile([FromBody] FileVerificationRequest request)
    {
        var courseWork = await _context.CourseWorks
            .FirstOrDefaultAsync(cw => cw.Id == Convert.ToInt32(request.FileId));

        if (courseWork == null)
            return NotFound($"Thesis with ID {request.FileId} not found.");

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

        var violations = VerifyFileContentByParameters(courseWork.FilePath, ruleParams);

        var courseWorkVerification = new CourseWorkVerification
        {
            CourseWorkId = courseWork.Id,
            VerificationDate = DateTime.UtcNow,
            Result = violations.Any() ? "Failed" : "Passed",
            Comments = violations.Any() ? "Some rules were violated" : "All rules passed",
            Violations = violations
        };

        _context.CourseWorkVerifications.Add(courseWorkVerification);
        await _context.SaveChangesAsync();

        //Повертаємо DTO
        var verificationResult = new FileVerificationResultDto
        {
            Id = courseWorkVerification.Id,
            FileUploadId = courseWork.Id,
            IsVerified = !violations.Any(),
            Violations = violations.Select(v => v.Description).ToList(),
            VerificationDate = courseWorkVerification.VerificationDate
        };

        return Ok(verificationResult);
    }

    private List<ViolationCourse> VerifyFileContentByParameters(string filePath, List<RuleParameter> ruleParams)
    {
        var violations = new List<ViolationCourse>();

        // Check if file exists
        if (!System.IO.File.Exists(filePath))
        {
            violations.Add(new ViolationCourse { Description = "File does not exist." });
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
                violations.Add(new ViolationCourse
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
                            violations.Add(new ViolationCourse
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
                                violations.Add(new ViolationCourse
                                {
                                    Description = $"File exceeds the maximum page limit of {maxPages}. Current pages: {pageCount}."
                                });
                            }
                        }
                        break;

                    case "BanWord":
                        if (content.Contains(parameterValue, StringComparison.OrdinalIgnoreCase))
                        {
                            violations.Add(new ViolationCourse
                            {
                                Description = $"The file contains a banned word: {parameterValue}."
                            });
                        }
                        break;

                    default:
                        violations.Add(new ViolationCourse
                        {
                            Description = $"Unknown rule parameter: {parameterName}."
                        });
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            violations.Add(new ViolationCourse
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
