using iTextSharp.text;
using iTextSharp.text.pdf;
using IncidentesFISEI.Application.Interfaces;

namespace IncidentesFISEI.Api.Services;

public class PdfGeneratorService
{
    private readonly IReportService _reportService;
    private readonly ILogger<PdfGeneratorService> _logger;

    public PdfGeneratorService(IReportService reportService, ILogger<PdfGeneratorService> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public async Task<byte[]> GenerateReportPdfAsync()
    {
        try
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 60, 50);
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // Obtener datos
            var performanceData = await _reportService.GetPerformanceReportAsync();
            var slaData = await _reportService.GetSLAReportAsync();
            var userActivityData = await _reportService.GetUserActivityReportAsync();

            // Título
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 24, Font.BOLD, new BaseColor(114, 47, 55));
            var title = new Paragraph("REPORTE DE INCIDENTES FISEI", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            document.Add(title);

            // Subtítulo con fecha
            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var subtitle = new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", subtitleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30
            };
            document.Add(subtitle);

            // Sección 1: Métricas de Rendimiento
            AddSection(document, "1. MÉTRICAS DE RENDIMIENTO");
            AddPerformanceTable(document, performanceData);

            // Sección 2: Cumplimiento SLA
            AddSection(document, "2. CUMPLIMIENTO SLA");
            AddSLATable(document, slaData);

            // Sección 3: Actividad de Usuarios
            AddSection(document, "3. ACTIVIDAD DE USUARIOS");
            AddUserActivityTable(document, userActivityData);

            // Footer
            var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            var footer = new Paragraph("Sistema de Gestión de Incidentes - FISEI", footerFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 30
            };
            document.Add(footer);

            document.Close();
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando PDF");
            throw;
        }
    }

    private void AddSection(Document document, string title)
    {
        var maroonColor = new BaseColor(114, 47, 55);
        var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, maroonColor);
        var sectionTitle = new Paragraph(title, sectionFont)
        {
            SpacingBefore = 15,
            SpacingAfter = 10
        };
        document.Add(sectionTitle);
    }

    private void AddPerformanceTable(Document document, PerformanceReportDto data)
    {
        var table = new PdfPTable(2) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1, 1 });
        table.SpacingBefore = 10;
        table.SpacingAfter = 20;

        // Header
        AddTableHeaderCell(table, "Métrica");
        AddTableHeaderCell(table, "Valor");

        // Datos
        var rowColor = true;
        AddTableDataCell(table, "Total de Incidentes", data.TotalIncidents.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Incidentes Resueltos", data.ResolvedIncidents.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Incidentes Pendientes", data.PendingIncidents.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Tiempo Promedio Resolución", $"{data.AverageResolutionTime:F2} horas", rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Última Actualización", data.UpdatedAt.ToString("dd/MM/yyyy HH:mm"), rowColor);

        document.Add(table);
    }

    private void AddSLATable(Document document, SLAReportDto data)
    {
        var table = new PdfPTable(2) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1, 1 });
        table.SpacingBefore = 10;
        table.SpacingAfter = 20;

        // Header
        AddTableHeaderCell(table, "Métrica");
        AddTableHeaderCell(table, "Valor");

        // Datos
        var rowColor = true;
        AddTableDataCell(table, "Total de Métricas SLA", data.TotalSLAMetric.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "SLA Cumplido", data.SLAMet.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "SLA Incumplido", data.SLABreached.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Cumplimiento %", $"{data.CompliancePercentage:F1}%", rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Última Actualización", data.UpdatedAt.ToString("dd/MM/yyyy HH:mm"), rowColor);

        document.Add(table);
    }

    private void AddUserActivityTable(Document document, UserActivityReportDto data)
    {
        var table = new PdfPTable(2) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1, 1 });
        table.SpacingBefore = 10;
        table.SpacingAfter = 20;

        // Header
        AddTableHeaderCell(table, "Métrica");
        AddTableHeaderCell(table, "Valor");

        // Datos
        var rowColor = true;
        AddTableDataCell(table, "Usuarios Activos (7 días)", data.ActiveUsers.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Total de Usuarios", data.TotalUsers.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Incidentes Creados Hoy", data.IncidentsCreatedToday.ToString(), rowColor);
        rowColor = !rowColor;
        AddTableDataCell(table, "Incidentes Resueltos Hoy", data.IncidentsResolvedToday.ToString(), rowColor);

        document.Add(table);

        // Top Usuarios Activos si existen
        if (data.TopActiveUsers?.Any() == true)
        {
            var maroonColor = new BaseColor(114, 47, 55);
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, Font.BOLD, maroonColor);
            var topTitle = new Paragraph("Usuarios más Activos:", titleFont)
            {
                SpacingBefore = 15,
                SpacingAfter = 10
            };
            document.Add(topTitle);

            var topUsersTable = new PdfPTable(2) { WidthPercentage = 100 };
            topUsersTable.SetWidths(new float[] { 2, 1 });
            topUsersTable.SpacingBefore = 10;
            topUsersTable.SpacingAfter = 20;

            // Header
            AddTableHeaderCell(topUsersTable, "Usuario");
            AddTableHeaderCell(topUsersTable, "Incidentes");

            // Datos
            rowColor = true;
            foreach (var user in data.TopActiveUsers.Take(10))
            {
                AddTableDataCell(topUsersTable, user.UserName ?? "Desconocido", user.IncidentCount.ToString(), rowColor);
                rowColor = !rowColor;
            }

            document.Add(topUsersTable);
        }
    }

    private void AddTableHeaderCell(PdfPTable table, string text)
    {
        var maroonColor = new BaseColor(114, 47, 55);
        var font = FontFactory.GetFont(FontFactory.HELVETICA, 11, Font.BOLD, BaseColor.WHITE);
        var cell = new PdfPCell(new Phrase(text, font))
        {
            BackgroundColor = maroonColor,
            Padding = 8,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE
        };
        table.AddCell(cell);
    }

    private void AddTableDataCell(PdfPTable table, string text, string value, bool alternateColor)
    {
        var bgColor = alternateColor ? new BaseColor(240, 240, 240) : BaseColor.WHITE;
        var font = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        var cellText = new PdfPCell(new Phrase(text, font))
        {
            BackgroundColor = bgColor,
            Padding = 6,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_MIDDLE
        };
        table.AddCell(cellText);

        var cellValue = new PdfPCell(new Phrase(value, font))
        {
            BackgroundColor = bgColor,
            Padding = 6,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_MIDDLE
        };
        table.AddCell(cellValue);
    }
}
