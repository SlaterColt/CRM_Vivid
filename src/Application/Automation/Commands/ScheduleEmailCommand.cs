using MediatR;

namespace CRM_Vivid.Application.Automation.Commands;

public record ScheduleEmailCommand(
    Guid ContactId,
    Guid TemplateId,
    DateTime? SendAt
) : IRequest<Unit>;