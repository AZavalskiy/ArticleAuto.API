using TFAuto.Domain;
using TFAuto.Domain.Repository.Roles;
using TFAuto.Domain.Seeds;
using TFAuto.WebApp;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Services
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<RoleInitializer>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

//Mappers
builder.Services.AddAutoMapper(typeof(UserMapper));

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

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
