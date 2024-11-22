using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Models;

public class FileUploadOptions
{
    public string Directory { get; set; }
    public string[] AllowedExtensions { get; set; }
    public long MaxFileSize { get; set; }
}
    