using System.ComponentModel.DataAnnotations;

namespace UniversityGrades.Models;

public class Course
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credits { get; set; }

    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
