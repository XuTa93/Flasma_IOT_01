using Flasma_IOT_01.Core.Models;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// In-memory data repository implementation
/// </summary>
public class InMemoryDataRepository
{
    private readonly List<Measurement> _measurements = new();
    private int _nextId = 1;

    public int Count => _measurements.Count;

    public void AddMeasurement(Measurement measurement)
    {
        measurement.Id = _nextId++;
        _measurements.Add(measurement);
    }

    public IEnumerable<Measurement> GetAllMeasurements()
    {
        return _measurements.AsReadOnly();
    }

    public IEnumerable<Measurement> GetMeasurementsByTimeRange(DateTime startTime, DateTime endTime)
    {
        return _measurements
            .Where(m => m.Timestamp >= startTime && m.Timestamp <= endTime)
            .ToList();
    }

    public void Clear()
    {
        _measurements.Clear();
        _nextId = 1;
    }
}
