using Flasma_IOT_01.Core.Models;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// Report exporter implementation for Excel and CSV formats
/// </summary>
public class ExcelReportExporter
{
    public Task ExportToExcelAsync(string filePath, IEnumerable<Measurement> measurements)
    {
        // TODO: Implement Excel export using EPPlus or similar library
        return Task.CompletedTask;
    }

    public Task ExportToCsvAsync(string filePath, IEnumerable<Measurement> measurements)
    {
        // TODO: Implement CSV export
        return Task.CompletedTask;
    }
}
