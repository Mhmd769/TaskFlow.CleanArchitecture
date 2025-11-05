using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskFlow.Application.Behaviors;
using TaskFlow.Application.Mappings;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// 🔹 Add Services to the Container
// =======================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================================
// 🔹 Database Context
// =======================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================================
// 🔹 Unit of Work
// =======================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =======================================
// 🔹 MediatR (CQRS Handlers)
// =======================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(AssemblyMarker).Assembly)); // ✅ ensures all Handlers are found

// =======================================
// 🔹 AutoMapper
// =======================================
builder.Services.AddAutoMapper(cfg =>
    cfg.AddMaps(typeof(UserMappingProfile).Assembly));

// =======================================
// 🔹 FluentValidation
// =======================================
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// =======================================
// 🔹 JWT Authentication
// =======================================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


// =======================================
// 🔹 Build App
// =======================================
var app = builder.Build();

// =======================================
// 🔹 Middleware Pipeline
// =======================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Important Order: Authentication → Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ Redirect root URL to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

// =======================================
// 🔹 Run App
// =======================================
app.Run();
