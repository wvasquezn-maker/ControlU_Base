using Microsoft.AspNetCore.Mvc;
using UniversityGrades.Controllers;
using UniversityGrades.DTOs;

namespace UniversityGrades.Tests;

public class ProfessorsControllerTests
{
    private ProfessorsController CreateController(string dbName)
    {
        var context = TestDbHelper.CreateEmptyContext(dbName);
        TestDbHelper.SeedMinimal(context);
        return new ProfessorsController(context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllProfessors()
    {
        var controller = CreateController(nameof(GetAll_ReturnsAllProfessors) + "Prof");

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var professors = Assert.IsAssignableFrom<IEnumerable<ProfessorResponseDto>>(ok.Value);
        Assert.Single(professors);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsProfessor()
    {
        var controller = CreateController(nameof(GetById_ExistingId_ReturnsProfessor) + "Prof");

        var result = await controller.GetById(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var professor = Assert.IsType<ProfessorResponseDto>(ok.Value);
        Assert.Equal("Test", professor.FirstName);
        Assert.Equal("Test Dept", professor.Department);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(GetById_NonExistingId_ReturnsNotFound) + "Prof");

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidProfessor_ReturnsCreatedAtAction()
    {
        var controller = CreateController(nameof(Create_ValidProfessor_ReturnsCreatedAtAction));
        var dto = new ProfessorCreateDto
        {
            FirstName = "Nuevo",
            LastName = "Profesor",
            Email = "nuevo@uni.edu",
            Department = "Matematicas"
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var professor = Assert.IsType<ProfessorResponseDto>(created.Value);
        Assert.Equal("Nuevo", professor.FirstName);
        Assert.Equal("Matematicas", professor.Department);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Update_ExistingId_ReturnsNoContent) + "Prof");
        var dto = new ProfessorCreateDto
        {
            FirstName = "Actualizado", LastName = "Prof",
            Email = "updated@uni.edu", Department = "New Dept"
        };

        var result = await controller.Update(10, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Update_NonExistingId_ReturnsNotFound) + "Prof");
        var dto = new ProfessorCreateDto
        {
            FirstName = "X", LastName = "Y",
            Email = "x@y.com", Department = "Z"
        };

        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var controller = CreateController(nameof(Delete_ExistingId_ReturnsNoContent) + "Prof");

        var result = await controller.Delete(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(nameof(Delete_NonExistingId_ReturnsNotFound) + "Prof");

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
