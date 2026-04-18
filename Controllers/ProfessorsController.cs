using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.DTOs;
using UniversityGrades.Models;

namespace UniversityGrades.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfessorsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessorResponseDto>>> GetAll()
    {
        var professors = await _context.Professors
            .Select(p => new ProfessorResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Department = p.Department
            })
            .ToListAsync();

        return Ok(professors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessorResponseDto>> GetById(int id)
    {
        var professor = await _context.Professors.FindAsync(id);
        if (professor == null) return NotFound();

        return Ok(new ProfessorResponseDto
        {
            Id = professor.Id,
            FirstName = professor.FirstName,
            LastName = professor.LastName,
            Email = professor.Email,
            Department = professor.Department
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProfessorResponseDto>> Create(ProfessorCreateDto dto)
    {
        var professor = new Professor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Department = dto.Department
        };

        _context.Professors.Add(professor);
        await _context.SaveChangesAsync();

        var response = new ProfessorResponseDto
        {
            Id = professor.Id,
            FirstName = professor.FirstName,
            LastName = professor.LastName,
            Email = professor.Email,
            Department = professor.Department
        };

        return CreatedAtAction(nameof(GetById), new { id = professor.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProfessorCreateDto dto)
    {
        var professor = await _context.Professors.FindAsync(id);
        if (professor == null) return NotFound();

        professor.FirstName = dto.FirstName;
        professor.LastName = dto.LastName;
        professor.Email = dto.Email;
        professor.Department = dto.Department;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var professor = await _context.Professors.FindAsync(id);
        if (professor == null) return NotFound();

        _context.Professors.Remove(professor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
