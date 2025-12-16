// FILE: src/Application/Common/Mappings/MappingProfile.cs (COMPLETE FILE)
using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Application.Tasks.Commands;
using CRM_Vivid.Application.Vendors.Commands;
using CRM_Vivid.Core.Enum;
using CRM_Vivid.Application.Notes.Commands;

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
                      .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()))
                      .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Name : null));

            CreateMap<CreateTaskCommand, Core.Entities.Task>();

            // --- NEW: PHASE 30 UPDATE TASK MAPPING FIX (Replaces manual GUID assignment in handler) ---
            CreateMap<UpdateTaskCommand, Core.Entities.Task>()
                // If the string field is used (ContactEmail), map the GUID field to null.
                // The handler logic will later resolve the correct GUID into the request.
                .ForMember(d => d.ContactId, opt => opt.MapFrom(s => s.ContactEmail != null ? (Guid?)null : s.ContactId))
                .ForMember(d => d.EventId, opt => opt.MapFrom(s => s.EventName != null ? (Guid?)null : s.EventId))
                .ForMember(d => d.VendorId, opt => opt.MapFrom(s => s.VendorName != null ? (Guid?)null : s.VendorId));

            // End UpdateTaskCommand Mapping Fix ---

            CreateMap<Vendor, VendorDto>()
                .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => s.ServiceType.ToString()));
            CreateMap<CreateVendorCommand, Vendor>()
                .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => Enum.Parse<VendorType>(s.ServiceType, true)));
            CreateMap<UpdateVendorCommand, Vendor>()
                .ForMember(d => d.ServiceType, opt => opt.MapFrom(s => Enum.Parse<VendorType>(s.ServiceType, true)));

            CreateMap<Note, NoteDto>()
                .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Name : null));

            CreateMap<CreateNoteCommand, Note>();
            CreateMap<UpdateNoteCommand, Note>()
                .ForMember(d => d.Content, opt => opt.MapFrom(s => s.Content));

            CreateMap<Template, TemplateDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

            // --- Financial Mappings ---
            CreateMap<Expense, ExpenseDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
                .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Name : null))
                .ForMember(d => d.LinkedDocumentName, opt => opt.MapFrom(s => s.LinkedDocument != null ? s.LinkedDocument.FileName : null));
        }
    }
}