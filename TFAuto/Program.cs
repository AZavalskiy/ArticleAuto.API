using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TFAuto.Domain;
using TFAuto.WebApp;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Services
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

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

app.UseExceptionHandler(error =>
{
    error.Run(async context =>
    {
        context.Response.ContentType = "text/plain";

        var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
        
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(validationException.Message);
        }
        else if (exception is Exception serverException)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(serverException.Message);
        }
        else
        {
            await context.Response.WriteAsync("An unknown error occurred");
        }
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
