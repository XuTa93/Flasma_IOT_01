using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;

namespace Flasma_IOT_01.Core.Examples;

/// <summary>
/// Examples for using CsvExporter
/// </summary>
public class CsvExporterExamples
{
    /// <summary>
    /// Basic CSV export example
    /// </summary>
    public static async Task BasicExportExample()
    {
        Console.WriteLine("=== BASIC CSV EXPORT EXAMPLE ===\n");

        // Create sample measurements
        var measurements = new List<Measurement>
        {
            new Measurement(220.0, 1.5),
            new Measurement(220.5, 1.52),
            new Measurement(219.8, 1.48),
            new Measurement(220.2, 1.51),
            new Measurement(220.1, 1.50)
        };

        await Task.Delay(100); // Small delay between measurements
        measurements.Add(new Measurement(220.3, 1.53));

        // Create exporter
        var exporter = new CsvExporter();

        // Generate filename
        var fileName = exporter.GenerateFileName("example");
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        // Export to file
        Console.WriteLine($"Exporting {measurements.Count} measurements to {fileName}...");
        await exporter.ExportToFileAsync(measurements, filePath);
        Console.WriteLine($"? Exported to: {filePath}\n");

        // Show statistics
        Console.WriteLine(exporter.GetStatisticsSummary(measurements));
    }

    /// <summary>
    /// Custom format export example
    /// </summary>
    public static async Task CustomFormatExample()
    {
        Console.WriteLine("\n=== CUSTOM FORMAT EXPORT EXAMPLE ===\n");

        // Create sample measurements
        var measurements = GenerateSampleMeasurements(5);

        var exporter = new CsvExporter();

        // Export with semicolon delimiter (for Excel compatibility in some regions)
        var fileName1 = "example_semicolon.csv";
        await exporter.ExportCustomFormatAsync(
            measurements,
            fileName1,
            delimiter: ";",
            dateFormat: "dd/MM/yyyy HH:mm:ss");
        Console.WriteLine($"? Exported with semicolon delimiter: {fileName1}");

        // Export without header
        var fileName2 = "example_no_header.csv";
        await exporter.ExportToFileAsync(
            measurements,
            fileName2,
            includeHeader: false);
        Console.WriteLine($"? Exported without header: {fileName2}");

        // Export without power column
        var fileName3 = "example_no_power.csv";
        await exporter.ExportToFileAsync(
            measurements,
            fileName3,
            includePower: false);
        Console.WriteLine($"? Exported without power column: {fileName3}");
    }

    /// <summary>
    /// Export to string example
    /// </summary>
    public static void ExportToStringExample()
    {
        Console.WriteLine("\n=== EXPORT TO STRING EXAMPLE ===\n");

        var measurements = GenerateSampleMeasurements(3);
        var exporter = new CsvExporter();

        // Export to string
        string csvContent = exporter.ExportToString(measurements);

        Console.WriteLine("CSV Content:");
        Console.WriteLine("????????????????????????????????????????????????");
        Console.WriteLine(csvContent);
        Console.WriteLine("????????????????????????????????????????????????");
    }

    /// <summary>
    /// Batch export example
    /// </summary>
    public static async Task BatchExportExample()
    {
        Console.WriteLine("\n=== BATCH EXPORT EXAMPLE ===\n");

        var exporter = new CsvExporter();

        // Export multiple datasets
        for (int i = 1; i <= 3; i++)
        {
            var measurements = GenerateSampleMeasurements(10);
            var fileName = exporter.GenerateFileName($"batch_{i}");

            await exporter.ExportToFileAsync(measurements, fileName);
            Console.WriteLine($"? Batch {i} exported: {fileName}");

            await Task.Delay(1100); // Ensure different timestamps
        }
    }

    /// <summary>
    /// Large dataset export example
    /// </summary>
    public static async Task LargeDatasetExample()
    {
        Console.WriteLine("\n=== LARGE DATASET EXPORT EXAMPLE ===\n");

        Console.WriteLine("Generating 1000 sample measurements...");
        var measurements = GenerateSampleMeasurements(1000);

        var exporter = new CsvExporter();
        var fileName = exporter.GenerateFileName("large_dataset");

        Console.WriteLine($"Exporting {measurements.Count} measurements...");
        var startTime = DateTime.Now;

        await exporter.ExportToFileAsync(measurements, fileName);

        var duration = (DateTime.Now - startTime).TotalMilliseconds;
        Console.WriteLine($"? Exported in {duration:F2}ms");
        Console.WriteLine($"? File: {fileName}");
        Console.WriteLine($"? File size: {new FileInfo(fileName).Length / 1024.0:F2} KB");
    }

    /// <summary>
    /// Statistics only example
    /// </summary>
    public static void StatisticsOnlyExample()
    {
        Console.WriteLine("\n=== STATISTICS ONLY EXAMPLE ===\n");

        var measurements = GenerateSampleMeasurements(50);
        var exporter = new CsvExporter();

        // Just get statistics without exporting
        string stats = exporter.GetStatisticsSummary(measurements);
        Console.WriteLine(stats);
    }

    /// <summary>
    /// Helper method to generate sample measurements
    /// </summary>
    private static List<Measurement> GenerateSampleMeasurements(int count)
    {
        var measurements = new List<Measurement>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            var voltage = 220.0 + (random.NextDouble() - 0.5) * 2; // 219-221V
            var current = 1.5 + (random.NextDouble() - 0.5) * 0.1; // 1.45-1.55A

            measurements.Add(new Measurement(voltage, current));

            // Small delay to create realistic timestamps
            Thread.Sleep(10);
        }

        return measurements;
    }

    /// <summary>
    /// Run all examples
    /// </summary>
    public static async Task RunAllExamples()
    {
        await BasicExportExample();
        await CustomFormatExample();
        ExportToStringExample();
        await BatchExportExample();
        await LargeDatasetExample();
        StatisticsOnlyExample();

        Console.WriteLine("\n=== ALL EXAMPLES COMPLETED ===");
    }
}
