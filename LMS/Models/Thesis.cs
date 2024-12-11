using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace LMS.Models;

public class Thesis
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }           
    public string FilePath { get; set; }            

    public int? AssignmentId { get; set; }
    public Assignment Assignment { get; set; }

    // Зв'язок з студентом (який подав дипломну роботу)
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }

    public string MentorId { get; set; }
    public ApplicationUser Mentor { get; set; }

    public ICollection<Rule> Rules { get; set; }
}
