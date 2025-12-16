using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Application.Common.Interfaces;

namespace CRM_Vivid.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
  private readonly ICurrentUserService? _currentUserService;
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null)
      : base(options)
  {
    _currentUserService = currentUserService;
  }

  public DbSet<Contact> Contacts { get; set; }
  public DbSet<Event> Events { get; set; }
  public DbSet<EventContact> EventContacts => Set<EventContact>();
  public DbSet<Note> Notes { get; set; } = null!;
  public DbSet<Core.Entities.Task> Tasks { get; set; }
  public DbSet<Vendor> Vendors { get; set; } = null!;
  public DbSet<Template> Templates { get; set; } = null!;
  public DbSet<EventVendor> EventVendors { get; set; } = null!;
  public DbSet<EmailLog> EmailLogs { get; set; } = null!;
  public DbSet<Document> Documents { get; set; }

  // --- NEW: The Ledger ---
  public DbSet<Budget> Budgets { get; set; }
  public DbSet<Expense> Expenses { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    if (_currentUserService != null) // Only apply filter if service is present
    {
      var scopedEntityTypes = modelBuilder.Model.GetEntityTypes()
          .Where(e => e.ClrType.GetInterfaces().Contains(typeof(IUserScopedEntity)));

      foreach (var entityType in scopedEntityTypes)
      {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "entity");

        var filter = System.Linq.Expressions.Expression.Lambda(
            System.Linq.Expressions.Expression.Equal(
                System.Linq.Expressions.Expression.Property(parameter, nameof(IUserScopedEntity.CreatedByUserId)),
                System.Linq.Expressions.Expression.Constant(_currentUserService.CurrentUserId)
            ),
            parameter
        );
        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
      }
    }

    base.OnModelCreating(modelBuilder);
  }
}