using System.ComponentModel.DataAnnotations;

namespace UniversityGrades.Models;

public class Grade
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    [Range(0, 100)]
    public decimal Score { get; set; }

    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTime GradeDate { get; set; }
}
