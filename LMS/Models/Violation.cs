namespace LMS.Models;

public class Violation
{
    public int Id { get; set; }
    public string Description { get; set; }

    public int ThesisVerificationId { get; set; }
    public ThesisVerification ThesisVerification { get; set; }
}
