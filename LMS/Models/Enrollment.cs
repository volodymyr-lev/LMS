﻿using Microsoft.AspNetCore.Identity;

namespace LMS.Models;

public class Enrollment
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; }              // Наприклад, "enrolled", "withdrawn"
}
