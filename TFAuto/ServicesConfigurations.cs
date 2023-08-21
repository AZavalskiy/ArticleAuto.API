using TFAuto.Domain;
using TFAuto.Domain.Seeds;
using TFAuto.Domain.Service.Configurations;
using TFAuto.Domain.Services.Blob;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.UserPassword;
using TFAuto.WebApp.Configurations;
using TFAuto.WebApp.Middleware;

namespace TFAuto.WebApp;

public static class ServicesConfigurations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddCosmosRepository(builder);
        AddBlobStorage(builder.Configuration);
        AddCors(builder);
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

    private static void AddBlobStorage(IConfiguration configuration)
    {
        configuration.GetConnectionString("BlobStorageConnectionString");
        configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>();
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
        serviceCollection.AddSwaggerGen();
    }

    private static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<IUserPasswordService, UserPasswordService>();
        serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
        serviceCollection.AddScoped<IRoleService, RoleService>();
        serviceCollection.AddScoped<RoleInitializer>();
        serviceCollection.AddScoped<IBlobService, BlobService>();
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
        }
    }

    public static void RegisterMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}