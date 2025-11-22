using System;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Common.Interfaces;

public interface IContractGenerator
{
  /// <summary>
  /// Generates a structured multi-page contract for a specific event.
  /// </summary>
  /// <param name="eventId">The ID of the event containing necessary data (timeline, contacts, financials).</param>
  /// <returns>The PDF document as a byte array.</returns>
  Task<byte[]> GenerateContractAsync(Guid eventId);
}