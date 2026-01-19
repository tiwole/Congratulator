using System.Reflection;
using System.Text.Json.Serialization;
using Congratulator.Core.Configurations;
using Congratulator.Core.Validators;
using Congratulator.Infrastructure.Configurations;
using Congratulator.Infrastructure.Data;
using Congratulator.Infrastructure.Extensions;
using Congratulator.Infrastructure.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var authContextConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString");
builder.Services.AddDbConfiguration<CongratulatorDbContext>(builder.Configuration, authContextConnectionString!);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCoreServices();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
                "https://localhost:8081",
                "https://api:80",
                "http://localhost:8081",
                "http://api:80",
                "https://localhost:7272", //temp
                "http://localhost:7272" //temp
            ) // Allowing requests from the Blazor app
            .AllowAnyMethod() // Allow GET, POST, OPTIONS, etc.
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            new JsonCamelCaseWithDotsNamingPolicy(); //Converts UserName.UserEmail to userName.userEmail
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonCamelCaseWithDotsNamingPolicy();
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Convert numbers to string for enum
    });

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Template API", Version = "v1"});

    var defaultXmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, defaultXmlFilename));
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.EnsureMigration<CongratulatorDbContext>(builder.Configuration);

app.UseCors("AllowBlazor");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(opts => { opts.RouteTemplate = "api/swagger/{documentname}/swagger.json"; });

    app.UseSwaggerUI(opts =>
    {
        opts.RoutePrefix = "api/swagger";
        opts.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Template v1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();