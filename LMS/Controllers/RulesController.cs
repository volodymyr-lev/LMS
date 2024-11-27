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
        var rules = await _context.Rules.Include(r => r.Parameters).ToListAsync();
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
            Parameters = new List<RuleParameter>() 
        };

        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRules), new { id = rule.Id }, rule);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] Rule updatedRule)
    {
        var rule = await _context.Rules.Include(r => r.Parameters).FirstOrDefaultAsync(r => r.Id == id);
        if (rule == null)
            return NotFound();

        rule.Name = updatedRule.Name;
        rule.Description = updatedRule.Description;

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
}
