using Journal_App.Data.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfColors = QuestPDF.Helpers.Colors;

namespace Journal_App.Services;

public class ExportService
{
    public ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportToPdfAsync(List<JournalEntry> entries)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(PdfColors.Grey.Darken4));

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(content => ComposeContent(content, entries));

                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        });
    }

    public async Task<byte[]> ExportSingleEntryToPdfAsync(JournalEntry entry)
    {
        return await ExportToPdfAsync(new List<JournalEntry> { entry });
    }

    private void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item()
                        .Text("Journal App")
                        .FontSize(24)
                        .Bold()
                        .FontColor(PdfColors.Grey.Darken4);

                    col.Item()
                        .Text($"Exported on {DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}")
                        .FontSize(10)
                        .FontColor(PdfColors.Grey.Medium);
                });
            });

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(PdfColors.Grey.Lighten2);
        });
    }

    private void ComposeContent(QuestPDF.Infrastructure.IContainer container, List<JournalEntry> entries)
    {
        container.PaddingVertical(15).Column(column =>
        {
            column.Spacing(20);

            column.Item()
                .Text($"{entries.Count} Journal {(entries.Count == 1 ? "Entry" : "Entries")}")
                .FontSize(14)
                .SemiBold()
                .FontColor(PdfColors.Grey.Darken3);

            foreach (var entry in entries.OrderByDescending(e => e.Date))
            {
                column.Item().Element(c => ComposeEntry(c, entry));
            }
        });
    }

    private void ComposeEntry(QuestPDF.Infrastructure.IContainer container, JournalEntry entry)
    {
        container
            .Border(1)
            .BorderColor(PdfColors.Grey.Lighten2)
            .Background(PdfColors.White)
            .Padding(20)
            .Column(column =>
            {
                column.Spacing(8);

                // Title
                column.Item()
                    .Text(entry.Title)
                    .FontSize(16)
                    .Bold()
                    .FontColor(PdfColors.Grey.Darken4);

                // Date and time row
                column.Item().Row(row =>
                {
                    row.AutoItem()
                        .Text(entry.Date.ToString("dddd, MMMM dd, yyyy"))
                        .FontSize(10)
                        .FontColor(PdfColors.Grey.Medium);

                    row.AutoItem()
                        .PaddingHorizontal(8)
                        .Text("|")
                        .FontSize(10)
                        .FontColor(PdfColors.Grey.Lighten1);

                    row.AutoItem()
                        .Text($"Created: {entry.CreatedAt:hh:mm tt}")
                        .FontSize(10)
                        .FontColor(PdfColors.Grey.Medium);

                    if (entry.HasBeenEdited)
                    {
                        row.AutoItem()
                            .PaddingHorizontal(8)
                            .Text("|")
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Lighten1);

                        row.AutoItem()
                            .Text($"Updated: {entry.UpdatedAt:hh:mm tt}")
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Medium);
                    }
                });

                // Moods
                column.Item().Row(row =>
                {
                    row.AutoItem()
                        .Text("Mood: ")
                        .FontSize(10)
                        .SemiBold()
                        .FontColor(PdfColors.Grey.Darken2);

                    row.AutoItem()
                        .Text(GetMoodText(entry))
                        .FontSize(10)
                        .FontColor(GetMoodColor(entry.PrimaryMood));
                });

                // Category
                if (!string.IsNullOrWhiteSpace(entry.Category))
                {
                    column.Item().Row(row =>
                    {
                        row.AutoItem()
                            .Text("Category: ")
                            .FontSize(10)
                            .SemiBold()
                            .FontColor(PdfColors.Grey.Darken2);

                        row.AutoItem()
                            .Text(entry.Category)
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Darken1);
                    });
                }

                // Tags
                if (!string.IsNullOrWhiteSpace(entry.Tags))
                {
                    column.Item().Row(row =>
                    {
                        row.AutoItem()
                            .Text("Tags: ")
                            .FontSize(10)
                            .SemiBold()
                            .FontColor(PdfColors.Grey.Darken2);

                        row.AutoItem()
                            .Text(entry.Tags)
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Darken1);
                    });
                }

                // Separator
                column.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(PdfColors.Grey.Lighten3);

                // Content
                column.Item()
                    .Text(entry.Content)
                    .FontSize(11)
                    .LineHeight(1.6f)
                    .FontColor(PdfColors.Grey.Darken3);

                // Word count
                column.Item()
                    .AlignRight()
                    .Text($"{entry.WordCount} words")
                    .FontSize(9)
                    .Italic()
                    .FontColor(PdfColors.Grey.Medium);
            });
    }

    private void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(0.5f).LineColor(PdfColors.Grey.Lighten2);

            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem()
                    .Text("Journal App - Personal Journal")
                    .FontSize(8)
                    .FontColor(PdfColors.Grey.Medium);

                row.RelativeItem()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span("Page ").FontSize(8).FontColor(PdfColors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(8).FontColor(PdfColors.Grey.Medium);
                        text.Span(" of ").FontSize(8).FontColor(PdfColors.Grey.Medium);
                        text.TotalPages().FontSize(8).FontColor(PdfColors.Grey.Medium);
                    });
            });
        });
    }

    private string GetMoodText(JournalEntry entry)
    {
        var moods = new List<string> { entry.PrimaryMood };

        if (!string.IsNullOrEmpty(entry.SecondaryMood1))
            moods.Add(entry.SecondaryMood1);

        if (!string.IsNullOrEmpty(entry.SecondaryMood2))
            moods.Add(entry.SecondaryMood2);

        return string.Join(", ", moods);
    }

    private string GetMoodColor(string mood)
    {
        var category = MoodCategories.GetMoodCategory(mood);
        return category switch
        {
            "Positive" => PdfColors.Green.Darken2,
            "Negative" => PdfColors.Red.Darken2,
            _ => PdfColors.Orange.Darken2
        };
    }
}
