namespace LMS.DTOs;

public class FileUploadDTO
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadDate { get; set; }
    public string UserId { get; set; }
    public string MentorOrAdvisorId { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
}