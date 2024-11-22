using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class CourseWork
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }          // Наприклад, "submitted", "approved", "rejected"
    public string FilePath { get; set; }        // Шлях до файлу курсової роботи

    // Зв'язок з студентом
    public string StudentId { get; set; }
    public IdentityUser Student { get; set; }
}
