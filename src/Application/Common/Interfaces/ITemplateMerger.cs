using CRM_Vivid.Application.Common.Models;

namespace CRM_Vivid.Application.Common.Interfaces;

public interface ITemplateMerger
{
  string Merge(string templateContent, ContactDto contact);
}