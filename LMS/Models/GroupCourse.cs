using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Models;

public class GroupCourse
{
    public int GroupId { get; set; }
    public Group Group { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }
}