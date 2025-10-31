using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Mappings;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using MediatR;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;

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

// 🔹 Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 🔹 Register AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserMappingProfile).Assembly));

// 🔹 Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateTaskCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateProjectCommandValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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

app.Run();
