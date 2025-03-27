using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest;
using FightingGameServer_Rest.Authorization;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.ApplicationServices;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using FightingGameServer_Rest.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ClosureAllocation

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 설정 함수들을 호출하여 서비스, 리포지토리, DB Context, 인증, 인가, Swagger 설정 
builder.ConfigurationApplicationServices();
builder.ConfigurationDataServices();
builder.ConfigureRepositories();
builder.ConfigureDbContext(builder.Configuration); // Configuration 객체를 넘겨줍니다. 
builder.ConfigureAuthentication();
builder.ConfigureAuthorization();
builder.ConfigureSwagger();
builder.ConfigureControllersAndCache(); // 컨트롤러 및 캐시 설정 추가 


WebApplication app = builder.Build();

// HTTP request pipeline 설정
app.ConfigureHttpRequestPipeline();

app.Run();

namespace FightingGameServer_Rest
{
    // --- 확장 메서드 정의 ---
    [SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
    internal static class ServiceCollectionExtensions
    {
        // 서비스 DI 설정 
        public static void ConfigurationApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICharacterInfoService, CharacterInfoService>();
            builder.Services.AddScoped<ICustomCommandManageService, CustomCommandManageService>();
            builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();
            builder.Services.AddScoped<IMatchRecordInfoService, MatchRecordInfoService>();
            builder.Services.AddScoped<IPlayerInfoService, PlayerInfoService>();
        }

        public static void ConfigurationDataServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<ICustomCommandService, CustomCommandService>();
            builder.Services.AddScoped<IMatchRecordService, MatchRecordService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<ISkillService, SkillService>();
            builder.Services.AddScoped<IUserService, UserService>();
        }

        // 리포지토리 DI 설정 
        public static void ConfigureRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
            builder.Services.AddScoped<ICustomCommandRepository, CustomCommandRepository>();
            builder.Services.AddScoped<IMatchRecordRepository, MatchRecordRepository>();
            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<ISkillRepository, SkillRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
        }

        // DB Context 설정 
        public static void ConfigureDbContext(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DevConnection");
            builder.Services.AddDbContext<GameDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString ??
                                                                            throw new InvalidOperationException(
                                                                                "Connection string is missing")));

                // 개발 환경 설정 
                // ReSharper disable once InvertIf
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
                }

                // **운영 환경 설정 주의 사항** 주석은 그대로 유지 
            });
        }

        // 인증 설정
        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication("JwtToken")
                .AddScheme<AuthenticationSchemeOptions, JwtTokenAuthenticationHandler>("JwtToken", _ => { });
        }

        // 인가 설정 
        public static void ConfigureAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Admin", policy => policy.Requirements.Add(new MinimumRoleRequirement(User.RoleType.Admin)))
                .AddPolicy("User", policy => policy.Requirements.Add(new MinimumRoleRequirement(User.RoleType.User)))
                .SetDefaultPolicy(new AuthorizationPolicyBuilder("JwtToken").RequireAuthenticatedUser().Build());
            builder.Services.AddSingleton<IAuthorizationHandler, MinimumRoleHandler>();
            builder.Services.AddSingleton<JwtTokenExtractor>();
        }

        // Swagger 설정 
        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Description =
                        "JWT Authorization header using the Bearer scheme. **Enter 'Bearer' [space] and then your token.** Example: \\\"Bearer 12345abcdef\\\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.SchemaFilter<EnumSchemaFilter>();
            });
        }

        // 컨트롤러 및 캐시 설정
        public static void ConfigureControllersAndCache(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
        }
    }

    internal static class ApplicationBuilderExtensions
    {
        // HTTP Request Pipeline 설정 
        public static void ConfigureHttpRequestPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();
            app.MapControllers();
        }
    }
}