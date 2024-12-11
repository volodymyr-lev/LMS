using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LMS.Models;
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }

    public string LecturerId { get; set; }
    public ApplicationUser Lecturer { get; set; }

    public string Syllabus { get; set; }

    public bool HasCourseWork { get; set; }
    public bool HasThesis { get; set; }

    public ICollection<Assignment> Assignments { get; set; }

    [JsonIgnore]
    public ICollection<GroupCourse> GroupCourses { get; set; }
}