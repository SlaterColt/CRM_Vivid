// src/Application/Vendors/Queries/GetEventsForVendorQuery.cs
using CRM_Vivid.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace CRM_Vivid.Application.Vendors.Queries
{
  public class GetEventsForVendorQuery : IRequest<List<EventDto>>
  {
    public Guid VendorId { get; set; }
  }
}