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
public class RulesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RulesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetRules()
    {
        var rules = await _context.Rules
                                   .Include(r => r.RuleParameters)
                                       .ThenInclude(rp => rp.RuleParameter) 
                                   .ToListAsync();

        return Ok(rules);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddRule([FromBody] CreateRuleDTO ruleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var rule = new Rule
        {
            Name = ruleDto.Name,
            Description = ruleDto.Description,
            RuleParameters = new List<RuleRuleParameter>()
        };

        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRules), new { id = rule.Id }, rule);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] Rule updatedRule)
    {
        var rule = await _context.Rules
                                 .Include(r => r.RuleParameters)
                                     .ThenInclude(rp => rp.RuleParameter)  
                                 .FirstOrDefaultAsync(r => r.Id == id);

        if (rule == null)
            return NotFound();

        rule.Name = updatedRule.Name;
        rule.Description = updatedRule.Description;

        if (updatedRule.RuleParameters != null && updatedRule.RuleParameters.Any())
        {
            foreach (var ruleRuleParameter in updatedRule.RuleParameters)
            {
                var existingParam = await _context.RuleParameters
                                                    .FirstOrDefaultAsync(rp => rp.Id == ruleRuleParameter.RuleParameterId);

                if (existingParam != null)
                {
                    ruleRuleParameter.RuleParameter = existingParam;
                    rule.RuleParameters.Add(ruleRuleParameter);
                }
            }
        }

        _context.Rules.Update(rule);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        var rule = await _context.Rules.FindAsync(id);
        if (rule == null)
            return NotFound();

        _context.Rules.Remove(rule);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/parameters/add")]
    public async Task<IActionResult> AddParameterToRule(int id, [FromBody] CreateRuleParameterDto parameterDto)
    {
        var rule = await _context.Rules.Include(r => r.RuleParameters).FirstOrDefaultAsync(r => r.Id == id);
        if (rule == null)
            return NotFound($"Правило з ID {id} не знайдено.");

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

        rule.RuleParameters.Add(ruleRuleParameter); 

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddParameterToRule), new { id = parameter.Id }, parameter);
    }

}
