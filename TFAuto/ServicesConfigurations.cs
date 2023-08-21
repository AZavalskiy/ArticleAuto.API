﻿using TFAuto.Domain;
using TFAuto.Domain.Seeds;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.UserPassword;
﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TFAuto.WebApp.Middleware;

namespace TFAuto.WebApp;

public static class ServicesConfigurations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddCosmosRepository(builder);
        AddCors(builder);
        ConfigureAuthentication(builder);
        AddSwagger(builder.Services);
        AddServices(builder.Services);
        AddMappers(builder.Services);
    }
    private static void AddCosmosRepository(WebApplicationBuilder builder)
    {
        builder.Services.AddCosmosRepository(options =>
        {
            options.CosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDBConnectionString");
            var cosmosSettings = builder.Configuration.GetSection("CosmosDBConnectionSettings").Get<CosmosDBConnectionSettings>();
            options.DatabaseId = cosmosSettings.DatabaseId;
            options.ContainerId = cosmosSettings.ContainerId;
        });
    }
    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }
    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Access Token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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
                    new List<string>()
                }
            });
        });
    }
    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var JWTSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = JWTSettings.ValidIssuer,
                ValidAudience = JWTSettings.ValidAudience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JWTSettings.IssuerSigningKey)
                    ),
                ValidateLifetime = true
            };
        });
    }
    private static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<IUserPasswordService, UserPasswordService>();
        serviceCollection.AddScoped<IRoleService, RoleService>();
        serviceCollection.AddScoped<RoleInitializer>();
        serviceCollection.AddScoped<PermissionInitializer>();
        serviceCollection.AddScoped<JWTService>();
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();

    }
    private static void AddMappers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(UserMapper));
        serviceCollection.AddAutoMapper(typeof(RoleUserMapper));
    }
    public static void InitializeSeeds(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
            roleInitializer.InitializeRoles().Wait();

            var permissionInitializer = scope.ServiceProvider.GetRequiredService<PermissionInitializer>();
            permissionInitializer.InitializePermissions().Wait();
        }
    }
    public static void RegisterMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}