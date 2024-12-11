using System.Text.Json.Serialization;

namespace LMS.Models;

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }

    public double? Score { get; set; }
    public string Status { get; set; }

    public int CourseId { get; set; }
    [JsonIgnore]
    public Course Course { get; set; }

    // Зв'язок із правилом
    public int RuleId { get; set; }
    public Rule Rule { get; set; }

    public ICollection<CourseWork> CourseWorks { get; set; }
    public ICollection<Thesis> Theses { get; set; }
}
