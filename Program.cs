using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SmartDepo_CaseStudy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TramManager>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Tram Planning API", Version = "v1" });
});

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tram Planning API v1");
    c.RoutePrefix = "";
});

var api = app.MapGroup("/api/v1");

api.MapPost("/trams/initialize",
        async (TramManager tm, InitializeRequest req) =>
        {
            var (success, msg) = await tm.InitializeAsync(req.N, req.C);
            return success ? Results.Ok(new { msg }) : Results.BadRequest(new { msg });
        })
    .WithName("InitializeTrams");

api.MapGet("/trams",
        (TramManager tm) => Results.Ok(tm.GetTrams()))
    .WithName("GetTrams");

api.MapPost("/trams/assign-mission",
        async (TramManager tm) =>
        {
            var (success, msg) = await tm.AssignMissionAsync();
            return success ? Results.Ok(new { msg }) : Results.Conflict(new { msg });
        })
    .WithName("AssignMission");

app.Run();

public record InitializeRequest(int N, int C);