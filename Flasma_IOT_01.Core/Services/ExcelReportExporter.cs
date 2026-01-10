using Flasma_IOT_01.Core.Models;
using System.Text;

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

    public async Task ExportToCsvAsync(string filePath, IEnumerable<Measurement> measurements)
    {
        var measurementList = measurements.ToList();
        
        if (!measurementList.Any())
        {
            throw new InvalidOperationException("No measurements to export");
        }

        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Id,Timestamp,Voltage (V),Current (A),Power (W)");
        
        // Data rows
        foreach (var measurement in measurementList)
        {
            var power = measurement.Voltage * measurement.Current;
            csv.AppendLine($"{measurement.Id},{measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff},{measurement.Voltage:F2},{measurement.Current:F2},{power:F2}");
        }

        // Ensure directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write to file
        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Generate default file path with timestamp
    /// </summary>
    public string GenerateDefaultFilePath(string? customFileName = null)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = customFileName ?? $"Measurement_{timestamp}.csv";

        // Save to Data folder in desktop application directory

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var dataFolder = Path.Combine(desktopPath, "Data");
        Directory.CreateDirectory(dataFolder);
        
        return Path.Combine(dataFolder, fileName);
    }
}
