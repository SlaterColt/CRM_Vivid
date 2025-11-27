using CRM_Vivid.Application.Common.Interfaces;
using System.Text;

namespace CRM_Vivid.Infrastructure.Services;

public class TemplateMerger : ITemplateMerger
{
  public string Merge(string templateContent, Dictionary<string, string> placeholders)
  {
    if (string.IsNullOrWhiteSpace(templateContent) || placeholders == null || placeholders.Count == 0)
    {
      return templateContent ?? string.Empty;
    }

    var sb = new StringBuilder(templateContent);

    foreach (var kvp in placeholders)
    {
      var key = kvp.Key;
      var value = kvp.Value ?? string.Empty;

      // Support {{Key}} (Standard Mustache/Handlebars style)
      sb.Replace($"{{{{{key}}}}}", value);

      // Support {Key} (Legacy style)
      sb.Replace($"{{{key}}}", value);
    }

    return sb.ToString();
  }
}