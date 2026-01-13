using System.Reflection;
using System.Text.Json.Serialization;
using Congratulator.Core.Configurations;
using Congratulator.Infrastructure.Configurations;
using Congratulator.Infrastructure.Data;
using Congratulator.Infrastructure.Extensions;
using Congratulator.Infrastructure.Middleware;
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
builder.Services.AddDbConfiguration<CongratulatorDbContext>(builder.Configuration, authContextConnectionString);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCoreServices();

builder.Services.AddProblemDetails();

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

//builder.Services.AddFluentValidationAutoValidation()
//    .AddFluentValidationClientsideAdapters();
//builder.Services.AddValidatorsFromAssemblyContaining<SendMessageValidator>();

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