using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace LMS.Models;
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }

    public string LecturerId { get; set; }
    public IdentityUser Lecturer { get; set; }

    public ICollection<GroupCourse> GroupCourses { get; set; }
}