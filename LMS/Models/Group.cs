using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public ICollection<IdentityUser> Students { get; set; }
}