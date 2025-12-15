using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using TaskFlow.API.Middlewares;
using TaskFlow.Application.Behaviors;
using TaskFlow.Application.Common;
using TaskFlow.Application.Mappings;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Messaging;
using TaskFlow.Infrastructure.Notifications;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.security;
using TaskFlow.Infrastructure.Seed;
using TaskFlow.Infrastructure.Services;

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



builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});


builder.Services.AddScoped<ICacheService, RedisCacheService>();

// 1️⃣ Add SignalR
builder.Services.AddSignalR();

// 2️⃣ Register your NotificationService (that uses IHubContext)
builder.Services.AddScoped<INotificationService, NotificationService>();


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

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();



builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddScoped<ITaskEventProducer, TaskEventProducer>();


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
    // 🔥🔥 IMPORTANT: Allow JWT tokens in WebSockets
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Allow the token in query string for SignalR
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            // Check if the request is for your hub
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
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


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
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

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// ✅ Important Order: CORS before auth for preflight and SignalR
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
// Ensure SignalR also respects CORS (map after middleware setup)

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");


// ✅ Redirect root URL to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

// ✅ Seed SuperAdmin (runs once if not exists)
await DatabaseSeeder.SeedSuperAdminAsync(app.Services);

// =======================================
// 🔹 Run App
// =======================================
app.Run();
