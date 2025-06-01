using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured")))
        };
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.Run();
