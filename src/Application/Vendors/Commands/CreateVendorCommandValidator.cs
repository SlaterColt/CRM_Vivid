// FILE: src/Application/Vendors/Commands/CreateVendorCommandValidator.cs
using FluentValidation;
using CRM_Vivid.Core.Enum;
using System.Text.Json; // NEW

namespace CRM_Vivid.Application.Vendors.Commands;

public class CreateVendorCommandValidator : AbstractValidator<CreateVendorCommand>
{
  public CreateVendorCommandValidator()
  {
    RuleFor(v => v.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

    RuleFor(v => v.Email)
        .EmailAddress().WithMessage("A valid email address is required.")
        .When(v => !string.IsNullOrEmpty(v.Email));

    RuleFor(v => v.ServiceType)
        .NotEmpty().WithMessage("ServiceType is required.")
        .IsEnumName(typeof(VendorType), caseSensitive: false)
        .WithMessage("A valid ServiceType is required.");

    // --- PHASE 28: CONDITIONAL JSON ATTRIBUTE VALIDATION ---
    RuleFor(v => v.Attributes)
        .Must(ContainRequiredAttributes)
        .WithMessage("If ServiceType is Venue, Attributes must contain a numeric 'Capacity'. If Entertainment, it must contain a 'Genre'.")
        .When(v => v.ServiceType.Equals("Venue", StringComparison.OrdinalIgnoreCase) ||
                   v.ServiceType.Equals("Entertainment", StringComparison.OrdinalIgnoreCase));
  }

  private bool ContainRequiredAttributes(CreateVendorCommand command, string? attributesJson)
  {
    if (string.IsNullOrWhiteSpace(attributesJson))
    {
      // If attributes are empty and type requires them, this is invalid.
      return false;
    }

    try
    {
      var json = JsonDocument.Parse(attributesJson);

      if (command.ServiceType.Equals("Venue", StringComparison.OrdinalIgnoreCase))
      {
        // Venue: Must have Capacity (must be a number)
        if (json.RootElement.TryGetProperty("Capacity", out var capacityElement) &&
            capacityElement.ValueKind == JsonValueKind.Number)
        {
          return capacityElement.GetInt32() > 0;
        }
        return false;
      }

      if (command.ServiceType.Equals("Entertainment", StringComparison.OrdinalIgnoreCase))
      {
        // Entertainment: Must have Genre (must be a non-empty string)
        if (json.RootElement.TryGetProperty("Genre", out var genreElement) &&
            genreElement.ValueKind == JsonValueKind.String)
        {
          return !string.IsNullOrWhiteSpace(genreElement.GetString());
        }
        return false;
      }

      return true; // If we reach here, it's a valid JSON for a non-specialized type
    }
    catch (JsonException)
    {
      return false; // Invalid JSON format
    }
    catch
    {
      return false; // Catch all other parsing/logic errors
    }
  }
}