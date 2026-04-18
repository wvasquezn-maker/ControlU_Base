using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.DTOs;
using UniversityGrades.Models;

namespace UniversityGrades.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrollmentsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetAll()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Select(e => new EnrollmentResponseDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student.FirstName + " " + e.Student.LastName,
                CourseId = e.CourseId,
                CourseName = e.Course.Name,
                EnrollmentDate = e.EnrollmentDate,
                Semester = e.Semester
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentResponseDto>> GetById(int id)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null) return NotFound();

        return Ok(new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student.FirstName + " " + enrollment.Student.LastName,
            CourseId = enrollment.CourseId,
            CourseName = enrollment.Course.Name,
            EnrollmentDate = enrollment.EnrollmentDate,
            Semester = enrollment.Semester
        });
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(EnrollmentCreateDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null) return BadRequest("El estudiante especificado no existe.");

        var course = await _context.Courses.FindAsync(dto.CourseId);
        if (course == null) return BadRequest("La materia especificada no existe.");

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Semester = dto.Semester
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        var response = new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = student.FirstName + " " + student.LastName,
            CourseId = enrollment.CourseId,
            CourseName = course.Name,
            EnrollmentDate = enrollment.EnrollmentDate,
            Semester = enrollment.Semester
        };

        return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EnrollmentCreateDto dto)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        enrollment.StudentId = dto.StudentId;
        enrollment.CourseId = dto.CourseId;
        enrollment.Semester = dto.Semester;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/grades")]
    public async Task<ActionResult<IEnumerable<GradeResponseDto>>> GetGrades(int id)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null) return NotFound();

        var grades = await _context.Grades
            .Where(g => g.EnrollmentId == id)
            .Select(g => new GradeResponseDto
            {
                Id = g.Id,
                EnrollmentId = g.EnrollmentId,
                StudentName = enrollment.Student.FirstName + " " + enrollment.Student.LastName,
                CourseName = enrollment.Course.Name,
                Score = g.Score,
                Description = g.Description,
                GradeDate = g.GradeDate
            })
            .ToListAsync();

        return Ok(grades);
    }
}
