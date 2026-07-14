using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Auth;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Repositories;
using RegistrosInstitucionales.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Acceso a datos: EF Core sobre SQL Server (Pregunta 4, requisito de acceso a datos).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Inyección de dependencias: Controller -> Service -> Repository (Pregunta 4).
builder.Services.AddScoped<IEntidadRepository, EntidadRepository>();
builder.Services.AddScoped<IRegistroRepository, RegistroRepository>();
builder.Services.AddScoped<ILogAccesoRepository, LogAccesoRepository>();
builder.Services.AddScoped<IRegistroConsultaService, RegistroConsultaService>();
builder.Services.AddScoped<ApiKeyAuthFilter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cualquier excepción no controlada se traduce a un 500 con ProblemDetails,
// en vez de filtrar detalles internos (stack trace) al cliente.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = app.Environment.IsDevelopment() ? feature?.Error.Message : "Ocurrió un error inesperado."
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para que el proyecto de tests (WebApplicationFactory u otros) pueda referenciar Program.
public partial class Program { }
