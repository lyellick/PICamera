using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using PICamera.Shared.Context;
using PICamera.Shared.ShemaFilters;
using PICamera.Service;
using PICamera.Shared.Extensions;
using PICamera.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHostedService<Worker>()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services
    .AddDbContext<StorageContext>(options => options.UseSqlite(builder.Configuration.TryGetValue("DefaultConnection", out string connectionString) ? connectionString : "Data Source=:memory:"))
    .AddScoped<IConfigurationService, ConfigurationService>()
    .AddScoped<ICameraService, CameraService>()
    .AddSwaggerGen(gen =>
    {
        gen.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "PICamera Service",
            Description = "PICamera Service Swagger Docs",
        });

        gen.SchemaFilter<EnumSchemaFilter>();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        gen.IncludeXmlComments(xmlPath);

        gen.AddSecurityDefinition("Admin Access Key", new OpenApiSecurityScheme()
        {
            Name = "admin-access-key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Description = "Authorization by admin-access-key inside request's header",
            Scheme = "ApiKeyScheme"
        });

        var admin = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Admin Access Key"
            },
            In = ParameterLocation.Header
        };

        gen.AddSecurityRequirement(new OpenApiSecurityRequirement { { admin, new List<string>() } });
    })
    .AddEndpointsApiExplorer()
    .AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1, 0); config.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddSingleton(builder.Configuration)
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer();



var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.UseCors(config =>
{
    config.AllowAnyOrigin();
    config.AllowAnyMethod();
    config.AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

IServiceScope scope = app.Services.CreateScope();
StorageContext context = scope.ServiceProvider.GetService<StorageContext>();
IMemoryCache cache = scope.ServiceProvider.GetService<IMemoryCache>();

await context.Database.EnsureCreatedAsync();

app.Run();