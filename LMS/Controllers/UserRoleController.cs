using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class UserRoleController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRoleController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Оновлення ролі користувача
    [HttpPost("update-role")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Перевірка чи роль існує
        var roleExist = await _roleManager.RoleExistsAsync(request.Role);
        if (!roleExist)
        {
            return BadRequest("Role does not exist");
        }

        // Видалення користувача з поточних ролей
        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return BadRequest("Failed to remove user from current roles");
        }

        // Додавання нової ролі
        var addResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!addResult.Succeeded)
        {
            return BadRequest("Failed to add user to the new role");
        }

        return Ok("Role updated successfully");
    }
}

public class UpdateRoleRequest
{
    public string UserId { get; set; }
    public string Role { get; set; }
}