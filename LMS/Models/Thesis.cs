using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class Thesis
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double? Score { get; set; }
    public string Status { get; set; }              // Наприклад, "submitted", "approved", "rejected"
    public string FilePath { get; set; }            // Шлях до файлу дипломної роботи

    // Зв'язок з студентом (який подав дипломну роботу)
    public string StudentId { get; set; }
    public IdentityUser Student { get; set; }

    public string MentorId { get; set; }
    public IdentityUser Mentor { get; set; }

    public ICollection<Rule> Rules { get; set; }
}
