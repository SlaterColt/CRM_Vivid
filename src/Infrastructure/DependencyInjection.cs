using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Infrastructure.Persistence;
using CRM_Vivid.Infrastructure.Services;
using Hangfire;
using Hangfire.Redis.StackExchange;

namespace CRM_Vivid.Infrastructure;

public static class DependencyInjection
{
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

    // 5. PDF Contract Generation (NEW)
    services.AddTransient<IContractGenerator, ContractGenerator>(); // <-- NEW SERVICE REGISTRATION

    // 6. Hangfire & Redis Configuration (Preserved)
    services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseRedisStorage(configuration.GetConnectionString("RedisConnection")));

    services.AddHangfireServer();

    // 7. File Storage
    services.AddScoped<IFileStorageService, LocalFileStorageService>();

    return services;
  }
}