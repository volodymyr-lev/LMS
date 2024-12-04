using LMS.Data;
using LMS.DTOs;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class RuleParametersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RuleParametersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetParameters(int ruleId)
    {
        var parameters = await _context.RuleRuleParameters
            .Where(rp => rp.RuleId == ruleId)
            .Include(rp => rp.RuleParameter) 
            .ToListAsync();

        if (parameters == null || !parameters.Any())
        {
            return NotFound($"Параметри для правила з ID {ruleId} не знайдено.");
        }

        return Ok(parameters.Select(rp => rp.RuleParameter));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddParameter([FromBody] CreateRuleParameterDto parameterDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var rule = await _context.Rules.FindAsync(int.Parse(parameterDto.RuleId));
        if (rule == null)
            return NotFound($"Правило з ID {parameterDto.RuleId} не знайдено.");

        var parameter = new RuleParameter
        {
            Name = parameterDto.Name,
            Value = parameterDto.Value
        };

        _context.RuleParameters.Add(parameter);
        await _context.SaveChangesAsync();

        var ruleRuleParameter = new RuleRuleParameter
        {
            RuleId = rule.Id,
            RuleParameterId = parameter.Id
        };

        _context.RuleRuleParameters.Add(ruleRuleParameter);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddParameter), new { id = parameter.Id }, parameter);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateParameter(int id, [FromBody] RuleParameter updatedParameter)
    {
        var parameter = await _context.RuleParameters.FindAsync(id);
        if (parameter == null)
            return NotFound();

        parameter.Name = updatedParameter.Name;
        parameter.Value = updatedParameter.Value;

        _context.RuleParameters.Update(parameter);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteParameter(int id)
    {
        var parameter = await _context.RuleParameters.FindAsync(id);
        if (parameter == null)
            return NotFound();

        _context.RuleParameters.Remove(parameter);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
