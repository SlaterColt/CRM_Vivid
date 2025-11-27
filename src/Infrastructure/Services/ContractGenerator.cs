using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;

// --- Entity Aliases ---
using Document = QuestPDF.Fluent.Document;

namespace CRM_Vivid.Infrastructure.Services;

public class ContractGenerator : IContractGenerator
{
  private readonly IApplicationDbContext _context;

  public ContractGenerator(IApplicationDbContext context)
  {
    _context = context;
  }

  // Task signature assumed to be clean now, relying on user's fix
  public System.Threading.Tasks.Task<byte[]> GenerateContractAsync(Guid eventId)
  {
    // 1. Fetch Composite Data (Simulating Run of Show Data Fetch)
    var eventEntity = _context.Events
        .Include(e => e.EventVendors)
        .ThenInclude(ev => ev.Vendor)
        .FirstOrDefault(e => e.Id == eventId);

    if (eventEntity == null)
    {
      throw new Exception($"Event with ID {eventId} not found for contract generation.");
    }

    var primaryVendor = eventEntity.EventVendors.FirstOrDefault()?.Vendor ?? new Vendor { Name = "TBD Vendor" };

    // 2. Define the Document Structure
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

                  // --- PAGE 1: Contact/Run of Show Summary ---
                  column.Item().Text("SECTION 1: EVENT AND PARTNER SUMMARY").Style(Styles.H1);
                  column.Item().Element(content => CreateRunOfShowSummary(content, eventEntity, primaryVendor));

                  column.Item().PageBreak();

                  // --- PAGE 2: LCD ENTERTAINMENT POLICIES ---
                  column.Item().Text("SECTION 2: LCD ENTERTAINMENT POLICIES").Style(Styles.H1);
                  column.Item().Element(content => CreatePolicySection(content, "VIVID/LCD General Policies", "Here lie the policies regarding payment terms, cancellation, insurance, and compliance with local ordinances.", Colors.Red.Medium)); // Call to policy section


                  column.Item().PageBreak();

                  // --- PAGE 3: Venue/Vendor Specific Policies ---
                  column.Item().Text($"SECTION 3: {primaryVendor.Name.ToUpper()} POLICIES").Style(Styles.H1);
                  column.Item().Element(content => CreatePolicySection(content, "External Vendor Policies", "This section contains venue-specific riders, load-in/out schedules, and technical requirements provided by the vendor.", Colors.Blue.Medium)); // Call to policy section


                  column.Item().PageBreak();

                  // --- LAST PAGE: Signature Blocks ---
                  column.Item().Text("SECTION 4: EXECUTION AND AGREEMENT").Style(Styles.H1);
                  column.Item().Element(content => CreateSignatureBlock(content, eventEntity, primaryVendor));
                });

            // Reusable Footer
            page.Footer().Text(x =>
                {
                  x.CurrentPageNumber().FontSize(10).SemiBold();
                  x.Span(" / ").FontSize(10).SemiBold();
                  x.TotalPages().FontSize(10).SemiBold();
                });
          });
    });

    // 3. Generate the document bytes
    byte[] pdfBytes = document.GeneratePdf();

    return System.Threading.Tasks.Task.FromResult(pdfBytes);
  }

  // --- PRIVATE COMPONENT CREATORS ---

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
            col.Item().Text("LCD ENTERTAINMENT").Style(Styles.H1);
            col.Item().Text("VIVID EVENT CONTRACT").Style(Styles.H2);
            col.Item().Text($"Draft Date: {DateTime.Now:MMMM dd, yyyy}").Style(Styles.Label);
          });

      row.ConstantItem(100).AlignRight().Text("VIVID").SemiBold().FontSize(30).FontColor(Colors.Red.Medium);
    });
  }

  private static void CreateRunOfShowSummary(IContainer container, Event eventEntity, Vendor primaryVendor)
  {
    container.Column(col =>
    {
      // Event Details
      col.Item().Text("1. EVENT OVERVIEW").Style(Styles.H2);
      col.Item().Table(table =>
          {
            table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn(1);
                  columns.RelativeColumn(2);
                });

            // Using Element(container => ...) delegate to correctly wrap table cell content.
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("Event Name:").Style(Styles.Label); }));
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text(eventEntity.Name).Style(Styles.Value); }));

            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("Start Date/Time:").Style(Styles.Label); }));
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text($"{eventEntity.StartDateTime:MM/dd/yyyy h:mm tt}").Style(Styles.Value); }));

            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("Venue/Location:").Style(Styles.Label); }));
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text(eventEntity.Location ?? "To Be Determined").Style(Styles.Value); }));

            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("Primary Partner:").Style(Styles.Label); }));
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text(primaryVendor.Name).Style(Styles.Value); }));

            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("Timeline Status:").Style(Styles.Label); }));
            table.Cell().Element(container => container.PaddingVertical(5).PaddingHorizontal(5).Column(col => { col.Item().Text("AGREED (Latest Version)").Style(Styles.Value.FontColor(Colors.Green.Darken2)); }));
          });
    });
  }

  private static void CreatePolicySection(IContainer container, string title, string content, string color)
  {
    container.Column(col =>
    {
      col.Item().Text($"1. {title}").Style(Styles.H2);

      // FIX: Separate BorderLeft and BorderColor to resolve Argument 3 error
      col.Item().PaddingTop(5).BorderLeft(1).BorderColor(color).PaddingLeft(10)
              .Text(content).Style(Styles.Value);

      col.Item().PaddingTop(15).Text("--- FULL POLICY TEXT GOES HERE ---").Style(Styles.Label.FontSize(9));
      col.Item().Text(Placeholders.LoremIpsum()).Style(Styles.Value.FontSize(9));
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
            // Signer 1: LCD/VIVID
            row.RelativeItem().Column(c =>
                {
                  c.Item().BorderBottom(1).PaddingBottom(5).Text(" ").Style(Styles.Value);
                  c.Item().PaddingTop(5).Text("For LCD Entertainment / VIVID (Authorized Signer)").Style(Styles.Label);
                  c.Item().Text($"Signature Date: {DateTime.Now:MM/dd/yyyy}").Style(Styles.Label);
                });

            row.ConstantItem(50); // Spacer

            // Signer 2: Partner/Vendor
            row.RelativeItem().Column(c =>
                {
                  c.Item().BorderBottom(1).PaddingBottom(5).Text(" ").Style(Styles.Value);
                  c.Item().PaddingTop(5).Text($"For {primaryVendor.Name} (Authorized Signer)").Style(Styles.Label);
                  c.Item().Text($"Signature Date: {DateTime.Now:MM/dd/yyyy}").Style(Styles.Label);
                });
          });
    });
  }
}