using TFAuto.Domain;
using TFAuto.Domain.Seeds;
using TFAuto.Domain.Services.Roles;
using TFAuto.WebApp;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Services
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<RoleInitializer>();
builder.Services.AddScoped<IRoleService, RoleService>();

//Mappers
builder.Services.AddAutoMapper(typeof(UserMapper));
builder.Services.AddAutoMapper(typeof(RoleUserMapper));

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

//CosmosDB
builder.Services.AddCosmosRepository(options =>
{
    options.CosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDBConnectionString");
    var cosmosSettings = builder.Configuration.GetSection("CosmosDBConnectionSettings").Get<CosmosDBConnectionSettings>();
    options.DatabaseId = cosmosSettings.DatabaseId;
    options.ContainerId = cosmosSettings.ContainerId;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
    await roleInitializer.InitializeRoles();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
