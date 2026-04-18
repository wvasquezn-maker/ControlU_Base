using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.DTOs;
using UniversityGrades.Models;

namespace UniversityGrades.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetAll()
    {
        var students = await _context.Students
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                StudentCode = s.StudentCode
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponseDto>> GetById(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        return Ok(new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            StudentCode = student.StudentCode
        });
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> Create(StudentCreateDto dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            StudentCode = dto.StudentCode
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var response = new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            StudentCode = student.StudentCode
        };

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StudentCreateDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.Email = dto.Email;
        student.StudentCode = dto.StudentCode;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/grades")]
    public async Task<ActionResult<IEnumerable<GradeResponseDto>>> GetGrades(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        var grades = await _context.Grades
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.Course)
            .Where(g => g.Enrollment.StudentId == id)
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

    [HttpGet("{id}/average")]
    public async Task<ActionResult<StudentAverageDto>> GetAverage(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Grades)
            .Where(e => e.StudentId == id)
            .ToListAsync();

        var courseAverages = enrollments
            .Where(e => e.Grades.Any())
            .Select(e => new CourseAverageDto
            {
                CourseName = e.Course.Name,
                Semester = e.Semester,
                Average = e.Grades.Average(g => g.Score),
                GradeCount = e.Grades.Count
            })
            .ToList();

        return Ok(new StudentAverageDto
        {
            StudentId = id,
            StudentName = student.FirstName + " " + student.LastName,
            GeneralAverage = courseAverages.Any() ? courseAverages.Average(c => c.Average) : 0,
            CourseAverages = courseAverages
        });
    }
}
