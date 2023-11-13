using Akros.Security.UserManager.Extension;
using Akros.Security.UserManager.Routes;
using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;

var corsPolicy = "CORSPolicy";
var version1 = new ApiVersion(1);
var version2 = new ApiVersion(2);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy(corsPolicy, builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add services to the container.
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

logger.Information($"Environmet : {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders |
                            HttpLoggingFields.ResponseBody |
                            HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.RequestBody;

    options.RequestHeaders.Add(HeaderNames.Accept);
    options.RequestHeaders.Add(HeaderNames.ContentType);
    options.RequestHeaders.Add("X-Correlation-Id");
    options.RequestHeaders.Add(HeaderNames.ContentEncoding);
    options.RequestHeaders.Add(HeaderNames.ContentLength);
});

// Add services to the container.
builder.Services.AddServiceExtension(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(version1)
                    .HasApiVersion(version2)
                    .ReportApiVersions()
                    .Build();

app.UseHttpLogging();


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(corsPolicy);

app.UseHttpsRedirection();

app.UseUserRoutes(versionSet);
app.UseRoleRoutes(versionSet);

app.Run();