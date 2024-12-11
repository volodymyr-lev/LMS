using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace LMS.Models;

public class CourseWork
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FilePath { get; set; }        // Шлях до файлу курсової роботи

    public int? AssignmentId { get; set; }
    [JsonIgnore]
    public Assignment Assignment { get; set; }

    // Зв'язок з студентом
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }

    public string AdvisorId { get; set; }
    public ApplicationUser Advisor { get; set; }

    public ICollection<Rule> Rules { get; set; }
}
