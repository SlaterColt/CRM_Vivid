namespace CRM_Vivid.Application.Common.Interfaces;

public interface ITemplateMerger
{
  /// <summary>
  /// Merges a template string with a dictionary of placeholders.
  /// Supports both {Key} and {{Key}} formats.
  /// </summary>
  /// <param name="templateContent">The raw HTML/Text template.</param>
  /// <param name="placeholders">Dictionary where Key is the placeholder name (e.g. "EventName") and Value is the replacement text.</param>
  /// <returns>The merged string.</returns>
  string Merge(string templateContent, Dictionary<string, string> placeholders);
}