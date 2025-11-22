using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CRM_Vivid.Application.Common.Interfaces; // FIXED: Correct namespace for IApplicationDbContext
using CRM_Vivid.Infrastructure.Persistence;
using CRM_Vivid.Infrastructure.Services;
using Hangfire;
using Hangfire.Redis.StackExchange; // Ensure Hangfire is referenced if we configure it here, otherwise remove if configured elsewhere.

namespace CRM_Vivid.Infrastructure;

public static class DependencyInjection
{
  // FIXED: Method name changed from 'AddInfrastructure' to 'AddInfrastructureServices' to match Program.cs
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    // 1. Database Configuration
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

    // 2. Bind the Interface to the Concrete Context
    services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

    // 3. Email Service (SendGrid)
    services.AddTransient<IEmailSender, SendGridEmailSender>();

    // 4. Template Merger
    services.AddTransient<ITemplateMerger, TemplateMerger>();

    // 5. Hangfire & Redis Configuration
    // (Ensuring this is present since Program.cs uses the Dashboard)
    services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseRedisStorage(configuration.GetConnectionString("RedisConnection"))); // Ensure "RedisConnection" exists in appsettings, or change to your Redis string.

    services.AddHangfireServer();

    services.AddScoped<IFileStorageService, LocalFileStorageService>();

    return services;
  }
}