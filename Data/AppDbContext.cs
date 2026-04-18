using Microsoft.EntityFrameworkCore;
using UniversityGrades.Models;

namespace UniversityGrades.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.CourseId, e.Semester })
            .IsUnique();

        // Seed data
        modelBuilder.Entity<Professor>().HasData(
            new Professor { Id = 1, FirstName = "Carlos", LastName = "García", Email = "cgarcia@universidad.edu", Department = "Ingeniería" },
            new Professor { Id = 2, FirstName = "María", LastName = "López", Email = "mlopez@universidad.edu", Department = "Ciencias" }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, FirstName = "Juan", LastName = "Pérez", Email = "jperez@est.universidad.edu", StudentCode = "EST-001" },
            new Student { Id = 2, FirstName = "Ana", LastName = "Martínez", Email = "amartinez@est.universidad.edu", StudentCode = "EST-002" }
        );

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Name = "Programación I", Code = "PRG-101", Credits = 4, ProfessorId = 1 },
            new Course { Id = 2, Name = "Cálculo I", Code = "MAT-101", Credits = 5, ProfessorId = 2 },
            new Course { Id = 3, Name = "Base de Datos", Code = "BDD-201", Credits = 4, ProfessorId = 1 }
        );

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment { Id = 1, StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2026, 1, 15), Semester = "2026-1" },
            new Enrollment { Id = 2, StudentId = 1, CourseId = 2, EnrollmentDate = new DateTime(2026, 1, 15), Semester = "2026-1" },
            new Enrollment { Id = 3, StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2026, 1, 15), Semester = "2026-1" }
        );

        modelBuilder.Entity<Grade>().HasData(
            new Grade { Id = 1, EnrollmentId = 1, Score = 85, Description = "Parcial 1", GradeDate = new DateTime(2026, 2, 15) },
            new Grade { Id = 2, EnrollmentId = 1, Score = 90, Description = "Parcial 2", GradeDate = new DateTime(2026, 3, 15) },
            new Grade { Id = 3, EnrollmentId = 2, Score = 78, Description = "Parcial 1", GradeDate = new DateTime(2026, 2, 15) },
            new Grade { Id = 4, EnrollmentId = 3, Score = 92, Description = "Parcial 1", GradeDate = new DateTime(2026, 2, 15) }
        );
    }
}
