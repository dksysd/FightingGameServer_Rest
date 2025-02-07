using FightingGameServer_Rest.Authorization;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Repository;
using FightingGameServer_Rest.Repository.Interfaces;
using FightingGameServer_Rest.Services;
using FightingGameServer_Rest.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();

// Add repositories to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add db context to the container.
string? connectionString = builder.Configuration.GetConnectionString("DevConnection");
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString ??
                                                                throw new InvalidOperationException(
                                                                    "Connection string is missing")));

    // 자세한 오류 메시지 활성화
    options.EnableDetailedErrors();
    // 민감 정보 로깅 활성화
    options.EnableSensitiveDataLogging();
    // 로깅 설정 (Console logger)
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
});

builder.Services.AddControllers();

// Add MemoryCache to the container.
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication("SessionToken")
    .AddScheme<AuthenticationSchemeOptions, SessionTokenAuthenticationHandler>("SessionToken", _ => { });

// 인가 서비스 등록
builder.Services.AddAuthorization(options =>
{
    // 기본 정책은 인증된 사용자임을 요구
    options.FallbackPolicy = options.DefaultPolicy;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("SessionToken", new OpenApiSecurityScheme
    {
        Name = "session-token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter the session token"
    });
    
    // Security Requirement 추가 (모든 API에 적용)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "SessionToken" }
            },
            new List<string>()
        }
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();