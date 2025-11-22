using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using System.Text;

namespace CRM_Vivid.Infrastructure.Services;

public class TemplateMerger : ITemplateMerger
{
  public string Merge(string templateContent, ContactDto contact)
  {
    if (string.IsNullOrWhiteSpace(templateContent))
    {
      return string.Empty;
    }

    var sb = new StringBuilder(templateContent);

    // 1. Support Single Braces (e.g., {FirstName}) - This is what you used
    sb.Replace("{FirstName}", contact.FirstName ?? string.Empty);
    sb.Replace("{LastName}", contact.LastName ?? string.Empty);
    sb.Replace("{Organization}", contact.Organization ?? string.Empty);
    sb.Replace("{Title}", contact.Title ?? string.Empty);

    // 2. Support Double Braces (e.g., {{FirstName}}) - Just in case
    sb.Replace("{{FirstName}}", contact.FirstName ?? string.Empty);
    sb.Replace("{{LastName}}", contact.LastName ?? string.Empty);
    sb.Replace("{{Organization}}", contact.Organization ?? string.Empty);
    sb.Replace("{{Title}}", contact.Title ?? string.Empty);

    return sb.ToString();
  }
}