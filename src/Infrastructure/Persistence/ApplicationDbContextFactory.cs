// FILE: src/Infrastructure/Persistence/ApplicationDbContextFactory.cs (FINAL, BUILD-SAFE VERSION)
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// No using CRM_Vivid.Api; needed here!

namespace CRM_Vivid.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  // WARNING: In a real environment, you must manually set the "UserSecretsId" in 
  // the Api project's .csproj file to match this string if the secrets were not 
  // generated automatically.
  private const string ApiUserSecretsId = "crm-vivid-api-secrets";

  public ApplicationDbContext CreateDbContext(string[] args)
  {
    var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Api");

    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(apiProjectPath)
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.Development.json", optional: true)
        .AddEnvironmentVariables()
        // CRITICAL FIX: Use AddUserSecrets(assemblyName) method, passing the 
        // secrets ID string directly. This avoids the need for the marker class.
        .AddUserSecrets(ApiUserSecretsId)
        .Build();

    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
      throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Ensure it is set in appsettings.json or user secrets.");
    }

    builder.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

    return new ApplicationDbContext(builder.Options);
  }
}