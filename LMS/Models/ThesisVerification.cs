namespace LMS.Models;

public class ThesisVerification
{
    public int Id { get; set; }
    public int ThesisId { get; set; }
    public Thesis Thesis { get; set; }

    public DateTime VerificationDate { get; set; }
    public string Result { get; set; } 
    public string Comments { get; set; }
    public ICollection<Violation> Violations { get; set; }
}

public class CourseWorkVerification
{
    public int Id { get; set; }
    public int CourseWorkId { get; set; }
    public CourseWork CourseWork { get; set; }

    public DateTime VerificationDate { get; set; }
    public string Result { get; set; }
    public string Comments { get; set; }
    public ICollection<ViolationCourse> Violations { get; set; }
}