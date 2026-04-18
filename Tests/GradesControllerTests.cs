using Microsoft.AspNetCore.Mvc;
using UniversityGrades.Controllers;
using UniversityGrades.DTOs;

namespace UniversityGrades.Tests;

public class GradesControllerTests
{
    private GradesController CreateController(string dbName)
    {
        var context = TestDbHelper.CreateEmptyContext(dbName);
        TestDbHelper.SeedMinimal(context);
        return new GradesController(context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllGrades()
    {
        var controller = CreateController(nameof(GetAll_ReturnsAllGrades) + "Grade");

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var grades = Assert.IsAssignableFrom<IEnumerable<GradeResponseDto>>(ok.Value);
        Assert.Single(grades);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsGradeWithDetails()
    {
        var controller = CreateController(nameof(GetById_ExistingId_ReturnsGradeWithDetails));

        var result = await controller.GetById(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var grade = Assert.IsType<GradeResponseDto>(ok.Value);
        Assert.Equal(85, grade.Score);
        Assert.Equal("Test Grade", grade.Description);
        Assert.Equal("Test Student", grade.StudentName);
        Assert.Equal("Test Course", grade.CourseName);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetById_NonExistingId_ReturnsNotFound) + "Grade");

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidGrade_ReturnsCreatedAtAction()
    {
        var controller = CreateController(nameof(Create_ValidGrade_ReturnsCreatedAtAction));
        var dto = new GradeCreateDto
        {
            EnrollmentId = 10,
            Score = 92,
            Description = "Parcial 2"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var grade = Assert.IsType<GradeResponseDto>(created.Value);
        Assert.Equal(92, grade.Score);
        Assert.Equal("Parcial 2", grade.Description);
        Assert.Equal("Test Student", grade.StudentName);
    }

    [Fact]
    public async Task Create_InvalidEnrollmentId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Create_InvalidEnrollmentId_ReturnsBadRequest));
        var dto = new GradeCreateDto
        {
            EnrollmentId = 999,
            Score = 80,
            Description = "Test"
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_BoundaryScore_Zero_Succeeds()
    {
        var controller = CreateController(nameof(Create_BoundaryScore_Zero_Succeeds));
        var dto = new GradeCreateDto
        {
            EnrollmentId = 10, Score = 0, Description = "Zero score"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var grade = Assert.IsType<GradeResponseDto>(created.Value);
        Assert.Equal(0, grade.Score);
    }

    [Fact]
    public async Task Create_BoundaryScore_Hundred_Succeeds()
    {
        var controller = CreateController(nameof(Create_BoundaryScore_Hundred_Succeeds));
        var dto = new GradeCreateDto
        {
            EnrollmentId = 10, Score = 100, Description = "Perfect score"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var grade = Assert.IsType<GradeResponseDto>(created.Value);
        Assert.Equal(100, grade.Score);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Update_ExistingId_ReturnsNoContent) + "Grade");
        var dto = new GradeCreateDto
        {
            EnrollmentId = 10, Score = 95, Description = "Updated"
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Update_NonExistingId_ReturnsNotFound) + "Grade");
        var dto = new GradeCreateDto
        {
            EnrollmentId = 10, Score = 50, Description = "X"
        };

        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_InvalidEnrollmentId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Update_InvalidEnrollmentId_ReturnsBadRequest));
        var dto = new GradeCreateDto
        {
            EnrollmentId = 999, Score = 50, Description = "X"
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Delete_ExistingId_ReturnsNoContent) + "Grade");

        var result = await controller.Delete(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_NonExistingId_ReturnsNotFound) + "Grade");

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_MultipleGrades_SameEnrollment_Succeeds()
    {
        var controller = CreateController(nameof(Create_MultipleGrades_SameEnrollment_Succeeds));

        await controller.Create(new GradeCreateDto
        {
            EnrollmentId = 10, Score = 70, Description = "Parcial 2"
        });
        await controller.Create(new GradeCreateDto
        {
            EnrollmentId = 10, Score = 95, Description = "Parcial 3"
        });

        var result = await controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var grades = Assert.IsAssignableFrom<IEnumerable<GradeResponseDto>>(ok.Value);
        Assert.Equal(3, grades.Count()); // 1 seed + 2 new
    }
}
