using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<GroupCourse> GroupCourses { get; set; }
    public ICollection<ApplicationUser> Students { get; set; }
}