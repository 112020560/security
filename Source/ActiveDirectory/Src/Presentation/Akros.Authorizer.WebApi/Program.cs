using Akros.Authorizer.Application;
using Akros.Authorizer.Infrastructure.ActiveDirectory;
using Akros.Authorizer.Infrastructure.Mongo;
using Akros.Authorizer.Infrastructure.Persistence;
using Akros.Authorizer.Infrastructure.Shared;
using Akros.Authorizer.WebApi.Extension;
using Akros.Authorizer.WebApi.Middlewares;
using Akros.Authorizer.WebApi.Routers;
using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;

var corsPolicy = "CORSPolicy";
var version1 = new ApiVersion(1);
var version2 = new ApiVersion(2);
var version3 = new ApiVersion(3);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

logger.Information($"Environmet : {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.Host.UseSerilog(logger);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders |
                            HttpLoggingFields.ResponseBody |
                            HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.RequestBody;

    options.RequestHeaders.Add(HeaderNames.Accept);
    options.RequestHeaders.Add(HeaderNames.ContentType);
    options.RequestHeaders.Add("X-KEY");
    options.RequestHeaders.Add(HeaderNames.ContentEncoding);
    options.RequestHeaders.Add(HeaderNames.ContentLength);
});

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureMongoPersistence(builder.Configuration);
builder.Services.ConfigureActiveDirectory(builder.Configuration);
builder.Services.ConfigureShared(builder.Configuration);

builder.Services.ConfigureApplication(builder.Configuration);

builder.Services.ConfigureCorsPolicy(corsPolicy);
builder.Services.ConfiguVersioningApi();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(version1)
                    .HasApiVersion(version2)
                    .HasApiVersion(version3)
                    .ReportApiVersions()
                    .Build();

app.UseSerilogRequestLogging();
app.UseHttpLogging();

// Configure the HTTP request pipeline.

//ROUTES
app.UseAuthenticateRoutes(versionSet);
app.UseDomainRoutes(versionSet);
app.UseUserRoutes(versionSet);

app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    var descriptions = app.DescribeApiVersions();
    foreach (var desc in descriptions)
    {
        var url = $"/swagger/{desc.GroupName}/swagger.json";
        var name = desc.GroupName.ToUpperInvariant();
        opts.SwaggerEndpoint(url, $"Authenticate API {name}");
    }
});

app.UseCors(corsPolicy);

///ERROR MIDDLEWARE
app.UseErrorHandler();

app.Run();
