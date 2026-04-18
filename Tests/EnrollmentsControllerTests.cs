using Microsoft.AspNetCore.Mvc;
using UniversityGrades.Controllers;
using UniversityGrades.DTOs;

namespace UniversityGrades.Tests;

public class EnrollmentsControllerTests
{
    private EnrollmentsController CreateController(string dbName)
    {
        var context = TestDbHelper.CreateEmptyContext(dbName);
        TestDbHelper.SeedMinimal(context);
        return new EnrollmentsController(context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllEnrollments()
    {
        var controller = CreateController(nameof(GetAll_ReturnsAllEnrollments) + "Enroll");

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var enrollments = Assert.IsAssignableFrom<IEnumerable<EnrollmentResponseDto>>(ok.Value);
        Assert.Single(enrollments);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsEnrollmentWithNames()
    {
        var controller = CreateController(nameof(GetById_ExistingId_ReturnsEnrollmentWithNames));

        var result = await controller.GetById(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var enrollment = Assert.IsType<EnrollmentResponseDto>(ok.Value);
        Assert.Equal("Test Student", enrollment.StudentName);
        Assert.Equal("Test Course", enrollment.CourseName);
        Assert.Equal("2026-1", enrollment.Semester);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetById_NonExistingId_ReturnsNotFound) + "Enroll");

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidEnrollment_ReturnsCreatedAtAction()
    {
        var context = TestDbHelper.CreateEmptyContext(nameof(Create_ValidEnrollment_ReturnsCreatedAtAction));
        TestDbHelper.SeedMinimal(context);
        // Add a second course for a new enrollment
        context.Courses.Add(new Models.Course
        {
            Id = 11, Name = "Course 2", Code = "C2-100",
            Credits = 3, ProfessorId = 10
        });
        context.SaveChanges();
        var controller = new EnrollmentsController(context);

        var dto = new EnrollmentCreateDto
        {
            StudentId = 10,
            CourseId = 11,
            Semester = "2026-2"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var enrollment = Assert.IsType<EnrollmentResponseDto>(created.Value);
        Assert.Equal("Test Student", enrollment.StudentName);
        Assert.Equal("Course 2", enrollment.CourseName);
        Assert.Equal("2026-2", enrollment.Semester);
    }

    [Fact]
    public async Task Create_InvalidStudentId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Create_InvalidStudentId_ReturnsBadRequest));
        var dto = new EnrollmentCreateDto
        {
            StudentId = 999, CourseId = 10, Semester = "2026-1"
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_InvalidCourseId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Create_InvalidCourseId_ReturnsBadRequest));
        var dto = new EnrollmentCreateDto
        {
            StudentId = 10, CourseId = 999, Semester = "2026-1"
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Update_ExistingId_ReturnsNoContent) + "Enroll");
        var dto = new EnrollmentCreateDto
        {
            StudentId = 10, CourseId = 10, Semester = "2026-2"
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Update_NonExistingId_ReturnsNotFound) + "Enroll");
        var dto = new EnrollmentCreateDto
        {
            StudentId = 10, CourseId = 10, Semester = "2026-1"
        };

        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Delete_ExistingId_ReturnsNoContent) + "Enroll");

        var result = await controller.Delete(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_NonExistingId_ReturnsNotFound) + "Enroll");

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetGrades_ExistingEnrollment_ReturnsGrades()
    {
        var controller = CreateController(nameof(GetGrades_ExistingEnrollment_ReturnsGrades));

        var result = await controller.GetGrades(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var grades = Assert.IsAssignableFrom<IEnumerable<GradeResponseDto>>(ok.Value);
        Assert.Single(grades);
        Assert.Equal(85, grades.First().Score);
    }

    [Fact]
    public async Task GetGrades_NonExistingEnrollment_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetGrades_NonExistingEnrollment_ReturnsNotFound));

        var result = await controller.GetGrades(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
