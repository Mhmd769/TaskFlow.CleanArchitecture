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
using TaskFlow.Infrastructure.security;
using TaskFlow.Infrastructure.Seed;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog;
using TaskFlow.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// 🔹 Add Services to the Container
// =======================================
builder.Services.AddControllers();

// =======================================
// 🔹 Swagger with JWT Auth Support
// =======================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskFlow API",
        Version = "v1",
        Description = "TaskFlow Backend API with Clean Architecture"
    });

    // 🔐 Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
        typeof(AssemblyMarker).Assembly));

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
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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

// 🔹 Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));


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

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// ✅ Important Order
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ Redirect root URL to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

// ✅ Seed SuperAdmin (runs once if not exists)
await DatabaseSeeder.SeedSuperAdminAsync(app.Services);

// =======================================
// 🔹 Run App
// =======================================
app.Run();
