using LMS.DTOs;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var userList = new List<UserWithRoleResponse>();

        foreach (var user in users)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(); 
            userList.Add(new UserWithRoleResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            });
        }

        return Ok(userList);
    }
}