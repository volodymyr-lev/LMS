using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.DTOs;

public class UpdateGroupRequest
{
    public string Name { get; set; }
    public List<string> StudentIds { get; set; }
}