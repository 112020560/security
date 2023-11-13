using Akros.Authorizer.Application.Dto;
using Akros.Authorizer.Application.Main.CQRS.Users.Queries;
using Akros.Autorizer.Services.WebApi.Extensions;
using Akros.Autorizer.Services.WebApi.Middlewares;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;

var version1 = new ApiVersion(1);
var version2 = new ApiVersion(2);

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddTransversalLayer();
builder.Services.AddBusinessLayer();
//builder.Services.AddRabbitMqInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApiVersioningExtension();
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

// Configure the HTTP request pipeline.
app.UseHttpLogging();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("api/v{version:apiVersion}/authenticate", async (IMediator mediator, UserLoggedDto userLoggedDto) =>
{
    return Results.Ok(await mediator.Send(new UserAuthenticateQuery(userLoggedDto)));
})
    .WithName("UserAdAuthenticate")
    .WithApiVersionSet(versionSet).MapToApiVersion(version1);

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapHealthChecks("/healthz");

app.Run();
