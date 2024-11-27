namespace LMS.DTOs;

public class FileVerificationResultDto
{
    public int Id { get; set; }
    public int FileUploadId { get; set; }
    public bool IsVerified { get; set; }
    public List<string> Violations { get; set; }
    public DateTime VerificationDate { get; set; }
}