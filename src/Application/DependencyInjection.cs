using System.Reflection;
using CRM_Vivid.Application.Common.Behaviours; // Add this
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_Vivid.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

      // Add the Validation Behaviour
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    });

    return services;
  }
}