using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;
using UniversityGrades.Models;

namespace UniversityGrades.Tests;

public static class TestDbHelper
{
    public static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public static AppDbContext CreateEmptyContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureDeleted();
        return context;
    }

    public static void SeedMinimal(AppDbContext context)
    {
        context.Professors.Add(new Professor
        {
            Id = 10, FirstName = "Test", LastName = "Prof",
            Email = "test@uni.edu", Department = "Test Dept"
        });

        context.Students.Add(new Student
        {
            Id = 10, FirstName = "Test", LastName = "Student",
            Email = "student@uni.edu", StudentCode = "TST-001"
        });

        context.Courses.Add(new Course
        {
            Id = 10, Name = "Test Course", Code = "TST-100",
            Credits = 3, ProfessorId = 10
        });

        context.Enrollments.Add(new Enrollment
        {
            Id = 10, StudentId = 10, CourseId = 10,
            EnrollmentDate = DateTime.UtcNow, Semester = "2026-1"
        });

        context.Grades.Add(new Grade
        {
            Id = 10, EnrollmentId = 10, Score = 85,
            Description = "Test Grade", GradeDate = DateTime.UtcNow
        });

        context.SaveChanges();
    }
}
