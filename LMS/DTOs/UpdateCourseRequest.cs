using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.DTOs;
public class UpdateCourseRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public string LecturerId { get; set; }
    public List<int> GroupIds { get; set; }
}