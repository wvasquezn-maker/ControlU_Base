using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.DTOs;
using UniversityGrades.Models;

namespace UniversityGrades.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAll()
    {
        var courses = await _context.Courses
            .Include(c => c.Professor)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Credits = c.Credits,
                ProfessorId = c.ProfessorId,
                ProfessorName = c.Professor.FirstName + " " + c.Professor.LastName
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponseDto>> GetById(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Professor)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        return Ok(new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            Code = course.Code,
            Credits = course.Credits,
            ProfessorId = course.ProfessorId,
            ProfessorName = course.Professor.FirstName + " " + course.Professor.LastName
        });
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponseDto>> Create(CourseCreateDto dto)
    {
        var professor = await _context.Professors.FindAsync(dto.ProfessorId);
        if (professor == null) return BadRequest("El profesor especificado no existe.");

        var course = new Course
        {
            Name = dto.Name,
            Code = dto.Code,
            Credits = dto.Credits,
            ProfessorId = dto.ProfessorId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var response = new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            Code = course.Code,
            Credits = course.Credits,
            ProfessorId = course.ProfessorId,
            ProfessorName = professor.FirstName + " " + professor.LastName
        };

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CourseCreateDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        var professor = await _context.Professors.FindAsync(dto.ProfessorId);
        if (professor == null) return BadRequest("El profesor especificado no existe.");

        course.Name = dto.Name;
        course.Code = dto.Code;
        course.Credits = dto.Credits;
        course.ProfessorId = dto.ProfessorId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
