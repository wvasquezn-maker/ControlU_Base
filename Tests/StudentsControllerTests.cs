using Microsoft.AspNetCore.Mvc;
using UniversityGrades.Controllers;
using UniversityGrades.DTOs;

namespace UniversityGrades.Tests;

public class StudentsControllerTests
{
    private StudentsController CreateController(string dbName)
    {
        var context = TestDbHelper.CreateEmptyContext(dbName);
        TestDbHelper.SeedMinimal(context);
        return new StudentsController(context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllStudents()
    {
        var controller = CreateController(nameof(GetAll_ReturnsAllStudents));

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var students = Assert.IsAssignableFrom<IEnumerable<StudentResponseDto>>(ok.Value);
        Assert.Single(students);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsStudent()
    {
        var controller = CreateController(nameof(GetById_ExistingId_ReturnsStudent));

        var result = await controller.GetById(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var student = Assert.IsType<StudentResponseDto>(ok.Value);
        Assert.Equal("Test", student.FirstName);
        Assert.Equal("Student", student.LastName);
        Assert.Equal("TST-001", student.StudentCode);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetById_NonExistingId_ReturnsNotFound));

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidStudent_ReturnsCreatedAtAction()
    {
        var controller = CreateController(nameof(Create_ValidStudent_ReturnsCreatedAtAction));
        var dto = new StudentCreateDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "nuevo@uni.edu",
            StudentCode = "TST-002"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var student = Assert.IsType<StudentResponseDto>(created.Value);
        Assert.Equal("Nuevo", student.FirstName);
        Assert.Equal("Estudiante", student.LastName);
        Assert.True(student.Id > 0);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Update_ExistingId_ReturnsNoContent));
        var dto = new StudentCreateDto
        {
            FirstName = "Actualizado",
            LastName = "Student",
            Email = "updated@uni.edu",
            StudentCode = "TST-001"
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Update_NonExistingId_ReturnsNotFound));
        var dto = new StudentCreateDto
        {
            FirstName = "X", LastName = "Y",
            Email = "x@y.com", StudentCode = "X-001"
        };

        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Delete_ExistingId_ReturnsNoContent));

        var result = await controller.Delete(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_NonExistingId_ReturnsNotFound));

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ThenGetById_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_ThenGetById_ReturnsNotFound));

        await controller.Delete(10);
        var result = await controller.GetById(10);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetGrades_ExistingStudent_ReturnsGrades()
    {
        var controller = CreateController(nameof(GetGrades_ExistingStudent_ReturnsGrades));

        var result = await controller.GetGrades(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var grades = Assert.IsAssignableFrom<IEnumerable<GradeResponseDto>>(ok.Value);
        Assert.Single(grades);
        var grade = grades.First();
        Assert.Equal(85, grade.Score);
        Assert.Equal("Test Course", grade.CourseName);
    }

    [Fact]
    public async Task GetGrades_NonExistingStudent_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetGrades_NonExistingStudent_ReturnsNotFound));

        var result = await controller.GetGrades(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAverage_ExistingStudent_ReturnsCorrectAverage()
    {
        var controller = CreateController(nameof(GetAverage_ExistingStudent_ReturnsCorrectAverage));

        var result = await controller.GetAverage(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var avg = Assert.IsType<StudentAverageDto>(ok.Value);
        Assert.Equal(10, avg.StudentId);
        Assert.Equal(85, avg.GeneralAverage);
        Assert.Single(avg.CourseAverages);
        Assert.Equal("Test Course", avg.CourseAverages[0].CourseName);
    }

    [Fact]
    public async Task GetAverage_NonExistingStudent_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetAverage_NonExistingStudent_ReturnsNotFound));

        var result = await controller.GetAverage(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAverage_StudentWithNoGrades_ReturnsZeroAverage()
    {
        var context = TestDbHelper.CreateEmptyContext(nameof(GetAverage_StudentWithNoGrades_ReturnsZeroAverage));
        context.Students.Add(new Models.Student
        {
            Id = 20, FirstName = "Sin", LastName = "Notas",
            Email = "sin@uni.edu", StudentCode = "TST-099"
        });
        context.SaveChanges();
        var controller = new StudentsController(context);

        var result = await controller.GetAverage(20);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var avg = Assert.IsType<StudentAverageDto>(ok.Value);
        Assert.Equal(0, avg.GeneralAverage);
        Assert.Empty(avg.CourseAverages);
    }
}
