using CRM_Vivid.Infrastructure;
using CRM_Vivid.Application;
using FluentValidation.AspNetCore;
using CRM_Vivid.Api.Middleware;
using Clerk.Net.AspNetCore.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Hangfire;

var _myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5179", // Your Swagger UI
                                             "http://localhost:3000", // Your future frontend
                                             "http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAuthentication(ClerkAuthenticationDefaults.AuthenticationScheme)
    .AddClerkAuthentication(options =>
    {
        options.Authority = builder.Configuration["Clerk:Authority"]!;
        options.AuthorizedParty = builder.Configuration["Clerk:AuthorizedParty"]!;

        if (string.IsNullOrEmpty(options.Authority))
            throw new InvalidOperationException("Clerk:Authority is not configured.");
        if (string.IsNullOrEmpty(options.AuthorizedParty))
            throw new InvalidOperationException("Clerk:AuthorizedParty is not configured.");
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build());

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(_myAllowSpecificOrigins);

// --- NEW: Enable Static Files (Resolves 404 on Downloads) ---
app.UseStaticFiles();
// ------------------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();

// The Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();