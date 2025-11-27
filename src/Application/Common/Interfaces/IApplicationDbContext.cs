using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Common.Interfaces;

public interface IApplicationDbContext
{
  DbSet<Contact> Contacts { get; }
  DbSet<Event> Events { get; }
  DbSet<EventContact> EventContacts { get; }
  DbSet<Core.Entities.Task> Tasks { get; }
  DbSet<Vendor> Vendors { get; }
  DbSet<Note> Notes { get; }
  DbSet<EventVendor> EventVendors { get; }
  DbSet<Template> Templates { get; }
  DbSet<EmailLog> EmailLogs { get; }
  DbSet<Document> Documents { get; }

  DbSet<Budget> Budgets { get; }
  DbSet<Expense> Expenses { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}