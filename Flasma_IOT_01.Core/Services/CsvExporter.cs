using Flasma_IOT_01.Core.Models;
using System.Globalization;
using System.Text;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// CSV exporter for measurement data
/// </summary>
public class CsvExporter
{
    /// <summary>
    /// Export measurements to CSV file
    /// </summary>
    public async Task ExportToFileAsync(
        IEnumerable<Measurement> measurements, 
        string filePath,
        bool includeHeader = true,
        bool includePower = true)
    {
        var sb = new StringBuilder();

        // Header
        if (includeHeader)
        {
            sb.AppendLine(includePower 
                ? "Id,Timestamp,Voltage (V),Current (A),Power (W)" 
                : "Id,Timestamp,Voltage (V),Current (A)");
        }

        // Data rows
        foreach (var m in measurements)
        {
            var timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var voltage = m.Voltage.ToString("F2", CultureInfo.InvariantCulture);
            var current = m.Current.ToString("F3", CultureInfo.InvariantCulture);

            if (includePower)
            {
                var power = (m.Voltage * m.Current).ToString("F2", CultureInfo.InvariantCulture);
                sb.AppendLine($"{m.Id},{timestamp},{voltage},{current},{power}");
            }
            else
            {
                sb.AppendLine($"{m.Id},{timestamp},{voltage},{current}");
            }
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Export measurements to CSV string
    /// </summary>
    public string ExportToString(
        IEnumerable<Measurement> measurements,
        bool includeHeader = true,
        bool includePower = true)
    {
        var sb = new StringBuilder();

        // Header
        if (includeHeader)
        {
            sb.AppendLine(includePower
                ? "Id,Timestamp,Voltage (V),Current (A),Power (W)"
                : "Id,Timestamp,Voltage (V),Current (A)");
        }

        // Data rows
        foreach (var m in measurements)
        {
            var timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var voltage = m.Voltage.ToString("F2", CultureInfo.InvariantCulture);
            var current = m.Current.ToString("F3", CultureInfo.InvariantCulture);

            if (includePower)
            {
                var power = (m.Voltage * m.Current).ToString("F2", CultureInfo.InvariantCulture);
                sb.AppendLine($"{m.Id},{timestamp},{voltage},{current},{power}");
            }
            else
            {
                sb.AppendLine($"{m.Id},{timestamp},{voltage},{current}");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export measurements with custom format
    /// </summary>
    public async Task ExportCustomFormatAsync(
        IEnumerable<Measurement> measurements,
        string filePath,
        string delimiter = ",",
        string dateFormat = "yyyy-MM-dd HH:mm:ss.fff")
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(string.Join(delimiter, new[]
        {
            "Id",
            "Timestamp",
            "Voltage (V)",
            "Current (A)",
            "Power (W)",
            "Energy (Wh)"
        }));

        DateTime? lastTimestamp = null;
        double cumulativeEnergy = 0;

        foreach (var m in measurements)
        {
            var timestamp = m.Timestamp.ToString(dateFormat, CultureInfo.InvariantCulture);
            var voltage = m.Voltage.ToString("F2", CultureInfo.InvariantCulture);
            var current = m.Current.ToString("F3", CultureInfo.InvariantCulture);
            var power = (m.Voltage * m.Current).ToString("F2", CultureInfo.InvariantCulture);

            // Calculate energy (simple approximation)
            if (lastTimestamp.HasValue)
            {
                var timeSpan = (m.Timestamp - lastTimestamp.Value).TotalHours;
                cumulativeEnergy += (m.Voltage * m.Current) * timeSpan;
            }
            lastTimestamp = m.Timestamp;

            var energy = cumulativeEnergy.ToString("F4", CultureInfo.InvariantCulture);

            sb.AppendLine(string.Join(delimiter, new[]
            {
                m.Id.ToString(),
                timestamp,
                voltage,
                current,
                power,
                energy
            }));
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Generate filename with timestamp
    /// </summary>
    public string GenerateFileName(string prefix = "measurements", string extension = "csv")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{prefix}_{timestamp}.{extension}";
    }

    /// <summary>
    /// Get statistics summary
    /// </summary>
    public string GetStatisticsSummary(IEnumerable<Measurement> measurements)
    {
        var list = measurements.ToList();
        if (!list.Any())
            return "No measurements available.";

        var voltages = list.Select(m => m.Voltage).ToList();
        var currents = list.Select(m => m.Current).ToList();
        var powers = list.Select(m => m.Voltage * m.Current).ToList();

        var sb = new StringBuilder();
        sb.AppendLine("=== MEASUREMENT STATISTICS ===");
        sb.AppendLine($"Total Samples: {list.Count}");
        sb.AppendLine($"Time Range: {list.First().Timestamp:yyyy-MM-dd HH:mm:ss} to {list.Last().Timestamp:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Duration: {(list.Last().Timestamp - list.First().Timestamp).TotalSeconds:F2} seconds");
        sb.AppendLine();
        sb.AppendLine("Voltage:");
        sb.AppendLine($"  Min: {voltages.Min():F2} V");
        sb.AppendLine($"  Max: {voltages.Max():F2} V");
        sb.AppendLine($"  Avg: {voltages.Average():F2} V");
        sb.AppendLine();
        sb.AppendLine("Current:");
        sb.AppendLine($"  Min: {currents.Min():F3} A");
        sb.AppendLine($"  Max: {currents.Max():F3} A");
        sb.AppendLine($"  Avg: {currents.Average():F3} A");
        sb.AppendLine();
        sb.AppendLine("Power:");
        sb.AppendLine($"  Min: {powers.Min():F2} W");
        sb.AppendLine($"  Max: {powers.Max():F2} W");
        sb.AppendLine($"  Avg: {powers.Average():F2} W");

        return sb.ToString();
    }
}
