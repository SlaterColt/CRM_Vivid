using CRM_Vivid.Infrastructure;
using CRM_Vivid.Application;
using CRM_Vivid.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Hangfire;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var _myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// ADDED: Enable PII Logging for debugging (Development only)
if (builder.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

// 1. CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5179",
                                             "http://localhost:3000",
                                             "http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// FIX: Switching to standard AddJwtBearer for reliable JWT validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Authority (Issuer) tells the middleware where to fetch the public keys (JWKS endpoint)
        options.Authority = builder.Configuration["Clerk:Authority"];

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Set to true if you define the Audience (Aud) claim
            ValidateLifetime = true,
            // The Authority URL automatically resolves the Issuer Signing Key via the JWKS endpoint
        };

        // OPTIONAL: Event handlers for debugging validation failures
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token Validation Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
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
    builder.Configuration.AddUserSecrets<Program>();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(_myAllowSpecificOrigins);

// --- NEW: Enable Static Files (Resolves 404 on Downloads) ---
app.UseStaticFiles();
// ------------------------------------------------------------

// Middleware MUST be in this order: Authentication THEN Authorization
app.UseAuthentication();
app.UseAuthorization();

// The Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();