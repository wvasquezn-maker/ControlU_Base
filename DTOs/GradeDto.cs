using System.ComponentModel.DataAnnotations;

namespace UniversityGrades.DTOs;

public class GradeCreateDto
{
    public int EnrollmentId { get; set; }

    [Range(0, 100)]
    public decimal Score { get; set; }

    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}

public class GradeResponseDto
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime GradeDate { get; set; }
}

public class StudentAverageDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal GeneralAverage { get; set; }
    public List<CourseAverageDto> CourseAverages { get; set; } = new();
}

public class CourseAverageDto
{
    public string CourseName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public decimal Average { get; set; }
    public int GradeCount { get; set; }
}
