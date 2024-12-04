using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class UserGroup
{
    public int UserId { get; set; }
    public IdentityUser User{ get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }
}

