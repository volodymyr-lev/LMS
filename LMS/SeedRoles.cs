using Microsoft.AspNetCore.Identity;
using LMS.Models;

public static class SeedRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Administrator", "Student", "Lecturer", "Assistant", "Mentor", "Advisor", "Unassigned" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var defaultAdmin = await userManager.FindByEmailAsync("admin@admin.com");
        if (defaultAdmin == null)
        {
            var adminUser = new IdentityUser { UserName = "admin@admin.com", Email = "admin@admin.com", PhoneNumber = "-" };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
}
