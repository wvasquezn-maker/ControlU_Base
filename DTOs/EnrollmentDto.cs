using System.ComponentModel.DataAnnotations;

namespace UniversityGrades.DTOs;

public class EnrollmentCreateDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    [Required, MaxLength(20)]
    public string Semester { get; set; } = string.Empty;
}

public class EnrollmentResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Semester { get; set; } = string.Empty;
}
