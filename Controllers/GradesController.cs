using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.DTOs;
using UniversityGrades.Models;

namespace UniversityGrades.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly AppDbContext _context;

    public GradesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GradeResponseDto>>> GetAll()
    {
        var grades = await _context.Grades
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Course)
            .Select(g => new GradeResponseDto
            {
                Id = g.Id,
                EnrollmentId = g.EnrollmentId,
                StudentName = g.Enrollment.Student.FirstName + " " + g.Enrollment.Student.LastName,
                CourseName = g.Enrollment.Course.Name,
                Score = g.Score,
                Description = g.Description,
                GradeDate = g.GradeDate
            })
            .ToListAsync();

        return Ok(grades);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GradeResponseDto>> GetById(int id)
    {
        var grade = await _context.Grades
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null) return NotFound();

        return Ok(new GradeResponseDto
        {
            Id = grade.Id,
            EnrollmentId = grade.EnrollmentId,
            StudentName = grade.Enrollment.Student.FirstName + " " + grade.Enrollment.Student.LastName,
            CourseName = grade.Enrollment.Course.Name,
            Score = grade.Score,
            Description = grade.Description,
            GradeDate = grade.GradeDate
        });
    }

    [HttpPost]
    public async Task<ActionResult<GradeResponseDto>> Create(GradeCreateDto dto)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId);

        if (enrollment == null) return BadRequest("La inscripción especificada no existe.");

        var grade = new Grade
        {
            EnrollmentId = dto.EnrollmentId,
            Score = dto.Score,
            Description = dto.Description,
            GradeDate = DateTime.UtcNow
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        var response = new GradeResponseDto
        {
            Id = grade.Id,
            EnrollmentId = grade.EnrollmentId,
            StudentName = enrollment.Student.FirstName + " " + enrollment.Student.LastName,
            CourseName = enrollment.Course.Name,
            Score = grade.Score,
            Description = grade.Description,
            GradeDate = grade.GradeDate
        };

        return CreatedAtAction(nameof(GetById), new { id = grade.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, GradeCreateDto dto)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) return NotFound();

        var enrollment = await _context.Enrollments.FindAsync(dto.EnrollmentId);
        if (enrollment == null) return BadRequest("La inscripción especificada no existe.");

        grade.EnrollmentId = dto.EnrollmentId;
        grade.Score = dto.Score;
        grade.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) return NotFound();

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
