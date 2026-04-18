using Microsoft.AspNetCore.Mvc;
using UniversityGrades.Controllers;
using UniversityGrades.DTOs;

namespace UniversityGrades.Tests;

public class CoursesControllerTests
{
    private CoursesController CreateController(string dbName)
    {
        var context = TestDbHelper.CreateEmptyContext(dbName);
        TestDbHelper.SeedMinimal(context);
        return new CoursesController(context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCourses()
    {
        var controller = CreateController(nameof(GetAll_ReturnsAllCourses) + "Course");

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var courses = Assert.IsAssignableFrom<IEnumerable<CourseResponseDto>>(ok.Value);
        Assert.Single(courses);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsCourseWithProfessorName()
    {
        var controller = CreateController(nameof(GetById_ExistingId_ReturnsCourseWithProfessorName));

        var result = await controller.GetById(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var course = Assert.IsType<CourseResponseDto>(ok.Value);
        Assert.Equal("Test Course", course.Name);
        Assert.Equal("TST-100", course.Code);
        Assert.Equal(3, course.Credits);
        Assert.Equal("Test Prof", course.ProfessorName);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetById_NonExistingId_ReturnsNotFound) + "Course");

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidCourse_ReturnsCreatedAtAction()
    {
        var controller = CreateController(nameof(Create_ValidCourse_ReturnsCreatedAtAction));
        var dto = new CourseCreateDto
        {
            Name = "Nueva Materia",
            Code = "NM-101",
            Credits = 4,
            ProfessorId = 10
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var course = Assert.IsType<CourseResponseDto>(created.Value);
        Assert.Equal("Nueva Materia", course.Name);
        Assert.Equal("Test Prof", course.ProfessorName);
    }

    [Fact]
    public async Task Create_InvalidProfessorId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Create_InvalidProfessorId_ReturnsBadRequest));
        var dto = new CourseCreateDto
        {
            Name = "Materia", Code = "X-100",
            Credits = 3, ProfessorId = 999
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Update_ExistingId_ReturnsNoContent) + "Course");
        var dto = new CourseCreateDto
        {
            Name = "Updated Course", Code = "UPD-100",
            Credits = 5, ProfessorId = 10
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Update_NonExistingId_ReturnsNotFound) + "Course");
        var dto = new CourseCreateDto
        {
            Name = "X", Code = "X", Credits = 1, ProfessorId = 10
        };

        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_InvalidProfessorId_ReturnsBadRequest()
    {
        var controller = CreateController(nameof(Update_InvalidProfessorId_ReturnsBadRequest));
        var dto = new CourseCreateDto
        {
            Name = "X", Code = "X", Credits = 1, ProfessorId = 999
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Delete_ExistingId_ReturnsNoContent) + "Course");

        var result = await controller.Delete(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_NonExistingId_ReturnsNotFound) + "Course");

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
