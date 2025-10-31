using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Mappings;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using MediatR;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskFlow.Application;
using TaskFlow.Application.Behaviors; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 🔹 Register MediatR (scan both API & Application)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(AssemblyMarker).Assembly)); // ✅ Ensures all Handlers are found

// 🔹 Register AutoMapper (scan all mapping profiles)
builder.Services.AddAutoMapper(cfg =>
    cfg.AddMaps(typeof(UserMappingProfile).Assembly));

// 🔹 Register FluentValidation (scan all validators in Application layer)
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(); // ✅ cleaner way

// 🔹 Add pipeline behavior for validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// 🔹 Enable FluentValidation auto validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 🔹 Redirect root URL to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
