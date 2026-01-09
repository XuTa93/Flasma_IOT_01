namespace Flasma_IOT_01.Core.Models;

/// <summary>
/// Represents a measurement with voltage, current, and timestamp
/// </summary>
public class Measurement
{
    public int Id { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public DateTime Timestamp { get; set; }

    public Measurement()
    {
        Timestamp = DateTime.UtcNow;
    }

    public Measurement(double voltage, double current)
        : this()
    {
        Voltage = voltage;
        Current = current;
    }

    public Measurement(int id, double voltage, double current, DateTime timestamp)
    {
        Id = id;
        Voltage = voltage;
        Current = current;
        Timestamp = timestamp;
    }
}
