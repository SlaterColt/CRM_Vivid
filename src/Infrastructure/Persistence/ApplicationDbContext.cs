using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Infrastructure.Persistence.Configurations;

namespace CRM_Vivid.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
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

    base.OnModelCreating(modelBuilder);
  }
}