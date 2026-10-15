using EncontreUmProfessor.Application.Interfaces;
using EncontreUmProfessor.Application.Services;
using EncontreUmProfessor.Infrastructure.Data;
using EncontreUmProfessor.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<IBuscaProfessorService, BuscaProfessorService>();

var pathJson = Path.Combine(AppContext.BaseDirectory, "Data", "professores.json");
builder.Services.AddSingleton<IProfessorRepository>(new ProfessorRepository(pathJson));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapProfessoresEndpoint();

app.Run();
