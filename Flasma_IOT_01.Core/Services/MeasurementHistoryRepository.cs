using Flasma_IOT_01.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// In-memory repository for measurement history records with CSV persistence
/// </summary>
public class MeasurementHistoryRepository
{
    private readonly List<MeasurementHistory> _history = new();
    private readonly CsvHistoryStorage _csvStorage;
    private int _nextId = 1;

    public MeasurementHistoryRepository()
    {
        _csvStorage = new CsvHistoryStorage();
        LoadHistoryFromStorage();
    }

    public MeasurementHistoryRepository(CsvHistoryStorage csvStorage)
    {
        _csvStorage = csvStorage ?? throw new ArgumentNullException(nameof(csvStorage));
        LoadHistoryFromStorage();
    }

    /// <summary>
    /// Load history from CSV storage
    /// </summary>
    private void LoadHistoryFromStorage()
    {
        try
        {
            var histories = _csvStorage.LoadHistoryAsync().GetAwaiter().GetResult();
            _history.AddRange(histories);
            
            if (_history.Any())
            {
                _nextId = _history.Max(h => h.Id) + 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load history: {ex.Message}");
        }
    }

    public async Task AddHistoryAsync(MeasurementHistory history)
    {
        history.Id = _nextId++;
        _history.Add(history);
        
        // Save to CSV file
        await _csvStorage.SaveHistoryAsync(history);
    }

    public IEnumerable<MeasurementHistory> GetAllHistory()
    {
        return _history.OrderByDescending(h => h.StartTime).ToList();
    }

    public MeasurementHistory? GetHistoryById(int id)
    {
        return _history.FirstOrDefault(h => h.Id == id);
    }

    public async Task ClearAsync()
    {
        _history.Clear();
        _nextId = 1;
        await _csvStorage.ClearHistoryAsync();
    }

    public int Count => _history.Count;

    public string GetStorageFilePath() => _csvStorage.GetHistoryFilePath();
}