using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContext<QuoteDb>(opt => opt.UseInMemoryDatabase("QuoteList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "fake-dotnet-api";
    config.Title = "fake-dotnet-api v1";
    config.Version = "v1";
});

builder.Logging.AddConsole();

builder.Services.AddHealthChecks().AddCheck<DefaultHealthCheck>("Default");
builder.Services.AddHealthChecks().AddDbContextCheck<QuoteDb>();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "fake-dotnet-api";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
});

AppEndpoints.Map(app, builder.Configuration.GetSection("Version"));

QuoteEndpoints.Map(app);

app.Run();
