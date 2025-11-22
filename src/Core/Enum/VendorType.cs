using System.Text.Json.Serialization;

namespace CRM_Vivid.Core.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VendorType
{
  Catering,
  Venue,
  Security,
  Entertainment,
  Florist,
  Photography,
  Videography,
  Other
}