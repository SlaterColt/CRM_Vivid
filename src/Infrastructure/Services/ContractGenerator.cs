using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

// --- Entity Aliases ---
using Document = QuestPDF.Fluent.Document;

namespace CRM_Vivid.Infrastructure.Services;

public class ContractGenerator : IContractGenerator
{
  private readonly IApplicationDbContext _context;

  // DIARA'S BRAND COLORS
  private static readonly string ElectricCoral = "#FF6F61";
  private static readonly string DeepTeal = "#004a99";

  public ContractGenerator(IApplicationDbContext context)
  {
    _context = context;
    QuestPDF.Settings.License = LicenseType.Community;
  }

  public async Task<byte[]> GenerateContractAsync(Guid eventId)
  {
    var eventEntity = await _context.Events
        .Include(e => e.EventVendors)
        .ThenInclude(ev => ev.Vendor)
        .FirstOrDefaultAsync(e => e.Id == eventId);

    if (eventEntity == null)
    {
      throw new Exception($"Event with ID {eventId} not found for contract generation.");
    }

    var primaryVendor = eventEntity.EventVendors.FirstOrDefault()?.Vendor ?? new Vendor { Name = "TBD Vendor" };

    var document = Document.Create(container =>
    {
      container.Page(page =>
          {
          page.Margin(50);
          page.Size(PageSizes.A4);

          page.Header().Element(header => CreateHeader(header, eventEntity));

          page.Content().Column(column =>
              {
              column.Spacing(25);

                // --- PAGE 1 ---
              column.Item().Text("SECTION 1: EVENT AND PARTNER SUMMARY").Style(Styles.H1);
              column.Item().Element(content => CreateRunOfShowSummary(content, eventEntity, primaryVendor));
              column.Item().PageBreak();

                // --- PAGE 2 ---
              column.Item().Text("SECTION 2: LCD ENTERTAINMENT POLICIES").Style(Styles.H1);
              column.Item().Element(content => CreatePolicySection(content, "VIVID/LCD General Policies", "Here lie the policies regarding payment terms, cancellation, insurance, and compliance with local ordinances.", ElectricCoral));
              column.Item().PageBreak();

                // --- PAGE 3 ---
              column.Item().Text($"SECTION 3: {primaryVendor.Name.ToUpper()} POLICIES").Style(Styles.H1);
              column.Item().Element(content => CreatePolicySection(content, "External Vendor Policies", "This section contains venue-specific riders, load-in/out schedules, and technical requirements provided by the vendor.", DeepTeal));
              column.Item().PageBreak();

                // --- LAST PAGE ---
              column.Item().Text("SECTION 4: EXECUTION AND AGREEMENT").Style(Styles.H1);
              column.Item().Element(content => CreateSignatureBlock(content, eventEntity, primaryVendor));
            });

          page.Footer().AlignRight().Text(x =>
              {
              x.CurrentPageNumber().FontSize(10).SemiBold().FontColor(Colors.Grey.Medium);
              x.Span(" / ").FontSize(10).SemiBold().FontColor(Colors.Grey.Medium);
              x.TotalPages().FontSize(10).SemiBold().FontColor(Colors.Grey.Medium);
            });
        });
    });

    return document.GeneratePdf();
  }

  private static class Styles
  {
    public static TextStyle H1 => TextStyle.Default.FontSize(14).SemiBold().FontColor(Colors.Grey.Darken3).Underline();
    public static TextStyle H2 => TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Grey.Darken2);
    public static TextStyle Label => TextStyle.Default.FontSize(10).SemiBold().FontColor(Colors.Grey.Medium);
    public static TextStyle Value => TextStyle.Default.FontSize(11).FontColor(Colors.Black);
  }

  private static void CreateHeader(IContainer container, Event eventEntity)
  {
    container.Row(row =>
    {
      row.RelativeItem().Column(col =>
          {
            // Fixed: Removed NoUnderline(), creating a new style without underline manually if needed, 
            // or just using H1 without Underline in definition if possible.
            // Assuming H1 has underline, we can just use Default for H1-like text without underline:
          col.Item().Text("LCD ENTERTAINMENT").FontSize(14).SemiBold().FontColor(Colors.Black);
          col.Item().Text("VIVID EVENT CONTRACT").Style(Styles.H2.FontColor(ElectricCoral));
          col.Item().Text($"Generated: {DateTime.Now:MMMM dd, yyyy}").Style(Styles.Label);
        });

      row.ConstantItem(100).AlignRight().Text("VIVID").SemiBold().FontSize(30).FontColor(ElectricCoral);
    });
  }

  private static void CreateRunOfShowSummary(IContainer container, Event eventEntity, Vendor primaryVendor)
  {
    container.Column(col =>
    {
      col.Item().Text("1. EVENT OVERVIEW").Style(Styles.H2);
      col.Item().Table(table =>
          {
          table.ColumnsDefinition(columns =>
              {
              columns.RelativeColumn(1);
              columns.RelativeColumn(2);
            });

          table.Cell().Padding(5).Text("Event Name:").Style(Styles.Label);
          table.Cell().Padding(5).Text(eventEntity.Name).Style(Styles.Value);

          table.Cell().Padding(5).Text("Start Date/Time:").Style(Styles.Label);
          table.Cell().Padding(5).Text($"{eventEntity.StartDateTime:MM/dd/yyyy h:mm tt}").Style(Styles.Value);

          table.Cell().Padding(5).Text("Venue/Location:").Style(Styles.Label);
          table.Cell().Padding(5).Text(eventEntity.Location ?? "To Be Determined").Style(Styles.Value);

          table.Cell().Padding(5).Text("Primary Partner:").Style(Styles.Label);
          table.Cell().Padding(5).Text(primaryVendor.Name).Style(Styles.Value);

          table.Cell().Padding(5).Text("Status:").Style(Styles.Label);
          table.Cell().Padding(5).Text("DRAFT / PENDING SIGNATURE").Style(Styles.Value.FontColor(ElectricCoral).Bold());
        });
    });
  }

  private static void CreatePolicySection(IContainer container, string title, string content, string colorHex)
  {
    container.Column(col =>
    {
      col.Item().Text($"Policy: {title}").Style(Styles.H2);
      col.Item().PaddingTop(5).BorderLeft(4).BorderColor(colorHex).PaddingLeft(10).Text(content).Style(Styles.Value);
      col.Item().PaddingTop(15).Text("--- ADDITIONAL TERMS ATTACHED ---").Style(Styles.Label.FontSize(9));
      col.Item().Text(Placeholders.LoremIpsum()).Style(Styles.Value.FontSize(9).FontColor(Colors.Grey.Medium));
    });
  }

  private static void CreateSignatureBlock(IContainer container, Event eventEntity, Vendor primaryVendor)
  {
    container.PaddingTop(50).Column(col =>
    {
      col.Item().Text("AGREED & ACCEPTED").Style(Styles.H2.Underline());
      col.Item().PaddingVertical(20).Text("By signing below, all parties confirm agreement with the Timeline and Policies attached.").Style(Styles.Value.FontSize(10));

      col.Item().Row(row =>
          {
          row.RelativeItem().Column(c =>
              {
              c.Item().BorderBottom(1).PaddingBottom(5).Text(" ").Style(Styles.Value);
              c.Item().PaddingTop(5).Text("For LCD Entertainment").Style(Styles.Label);
              c.Item().Text($"Date: {DateTime.Now:MM/dd/yyyy}").Style(Styles.Label);
            });

          row.ConstantItem(50);

          row.RelativeItem().Column(c =>
              {
              c.Item().BorderBottom(1).PaddingBottom(5).Text(" ").Style(Styles.Value);
              c.Item().PaddingTop(5).Text($"For {primaryVendor.Name}").Style(Styles.Label);
              c.Item().Text("Date: _____________").Style(Styles.Label);
            });
        });
    });
  }
}