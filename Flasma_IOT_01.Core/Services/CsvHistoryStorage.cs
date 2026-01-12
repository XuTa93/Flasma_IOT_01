using Flasma_IOT_01.Core.Models;
using System.Text;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// Service for storing and loading measurement history to/from CSV file
/// </summary>
public class CsvHistoryStorage
{
    private readonly string _historyFilePath;

    public CsvHistoryStorage()
    {
        // Save history CSV to desktop
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var dataFolder = Path.Combine(desktopPath, "Data");
        Directory.CreateDirectory(dataFolder);
        _historyFilePath = Path.Combine(dataFolder, "MeasurementHistory.csv");
    }

    /// <summary>
    /// Save a history record to CSV file
    /// </summary>
    public async Task SaveHistoryAsync(MeasurementHistory history)
    {
        var fileExists = File.Exists(_historyFilePath);

        var csv = new StringBuilder();

        // Write header if file doesn't exist
        if (!fileExists)
        {
            csv.AppendLine("Id,StartTime,EndTime,Duration,Barcode,Result,TotalMeasurements,AverageVoltage,AverageCurrent,FilePath");
        }

        // Append history record
        csv.AppendLine($"{history.Id}," +
                      $"{history.StartTime:yyyy-MM-dd HH:mm:ss}," +
                      $"{history.EndTime:yyyy-MM-dd HH:mm:ss}," +
                      $"{history.Duration.TotalSeconds:F2}," +
                      $"\"{history.Barcode}\"," +
                      $"{history.Result}," +
                      $"{history.TotalMeasurements}," +
                      $"{history.AverageVoltage:F2}," +
                      $"{history.AverageCurrent:F2}," +
                      $"\"{history.FilePath}\"");

        await File.AppendAllTextAsync(_historyFilePath, csv.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Load all history records from CSV file
    /// </summary>
    public async Task<List<MeasurementHistory>> LoadHistoryAsync()
    {
        var histories = new List<MeasurementHistory>();

        if (!File.Exists(_historyFilePath))
            return histories;

        try
        {
            var lines = await File.ReadAllLinesAsync(_historyFilePath, Encoding.UTF8);

            // Skip header line
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var history = ParseHistoryLine(line);
                if (history != null)
                {
                    histories.Add(history);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error or handle as needed
            Console.WriteLine($"Error loading history: {ex.Message}");
        }

        return histories;
    }

    /// <summary>
    /// Parse a CSV line into MeasurementHistory object
    /// </summary>
    private MeasurementHistory? ParseHistoryLine(string line)
    {
        try
        {
            // Simple CSV parser (handles quoted fields)
            var values = ParseCsvLine(line);

            if (values.Length < 10)
                return null;

            return new MeasurementHistory
            {
                Id = int.Parse(values[0]),
                StartTime = DateTime.Parse(values[1]),
                EndTime = DateTime.Parse(values[2]),
                // Duration is calculated from StartTime and EndTime
                Barcode = values[4],
                Result = values[5],
                TotalMeasurements = int.Parse(values[6]),
                AverageVoltage = double.Parse(values[7]),
                AverageCurrent = double.Parse(values[8]),
                FilePath = values[9]
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Parse CSV line handling quoted fields
    /// </summary>
    private string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var currentValue = new StringBuilder();
        var insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        values.Add(currentValue.ToString());
        return values.ToArray();
    }

    /// <summary>
    /// Clear all history records
    /// </summary>
    public async Task ClearHistoryAsync()
    {
        if (File.Exists(_historyFilePath))
        {
            File.Delete(_historyFilePath);
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// Get the history file path
    /// </summary>
    public string GetHistoryFilePath() => _historyFilePath;
}