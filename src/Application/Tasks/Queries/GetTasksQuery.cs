// src/Application/Tasks/Queries/GetTasksQuery.cs
using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTasksQuery : IRequest<IEnumerable<TaskDto>>
  {
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }
    // NEW: Add VendorId to allow filtering by vendor
    public Guid? VendorId { get; set; }
  }
}