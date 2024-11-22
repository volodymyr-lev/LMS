using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Invalid course data");
            }

            var lecturer = await _userManager.FindByIdAsync(course.LecturerId);
            if (lecturer == null)
            {
                return BadRequest("Lecturer not found");
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateCourse), new { id = course.Id }, course);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses.Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            return Ok(course);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            if (updatedCourse == null || updatedCourse.Id != id)
            {
                return BadRequest("Invalid course data");
            }

            var existingCourse = await _context.Courses.Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.Id == id);
            if (existingCourse == null)
            {
                return NotFound("Course not found");
            }

            existingCourse.Name = updatedCourse.Name;
            existingCourse.Description = updatedCourse.Description;
            existingCourse.Credits = updatedCourse.Credits;
            existingCourse.LecturerId = updatedCourse.LecturerId;

            await _context.SaveChangesAsync();

            return Ok(existingCourse);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
