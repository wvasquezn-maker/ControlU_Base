using System.ComponentModel.DataAnnotations;

namespace UniversityGrades.DTOs;

public class CourseCreateDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credits { get; set; }

    public int ProfessorId { get; set; }
}

public class CourseResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
}
