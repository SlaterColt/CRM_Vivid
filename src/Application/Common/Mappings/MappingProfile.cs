using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Application.Tasks.Commands;
using CRM_Vivid.Application.Vendors.Commands;
using CRM_Vivid.Core.Enum;
using CRM_Vivid.Application.Notes.Commands;
using CRM_Vivid.Application.Templates.Commands;
using System;

namespace CRM_Vivid.Application.Common.Mappings
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<Contact, ContactDto>();
      CreateMap<Event, EventDto>();
      CreateMap<TaskDto, Core.Entities.Task>();

      CreateMap<Core.Entities.Task, TaskDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()));

      CreateMap<CreateTaskCommand, Core.Entities.Task>();
      CreateMap<UpdateTaskCommand, Core.Entities.Task>();

      CreateMap<Vendor, VendorDto>()
          .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => s.ServiceType.ToString()));

      CreateMap<CreateVendorCommand, Vendor>()
          .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => Enum.Parse<VendorType>(s.ServiceType, true)));

      CreateMap<UpdateVendorCommand, Vendor>()
          .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => Enum.Parse<VendorType>(s.ServiceType, true)));

      CreateMap<Note, NoteDto>();
      CreateMap<CreateNoteCommand, Note>();
      CreateMap<UpdateNoteCommand, Note>()
          .ForMember(d => d.Content, opt => opt.MapFrom(s => s.Content));

      CreateMap<Template, TemplateDto>()
          .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

      // --- NEW: Financial Mappings ---
      CreateMap<Expense, ExpenseDto>()
          .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
          .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Name : null))
          .ForMember(d => d.LinkedDocumentName, opt => opt.MapFrom(s => s.LinkedDocument != null ? s.LinkedDocument.FileName : null));
    }
  }
}