using Microsoft.EntityFrameworkCore;
using UniversityGrades.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "University Grades API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UniversityGradesDb"));

var app = builder.Build();

// Ensure seed data is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
