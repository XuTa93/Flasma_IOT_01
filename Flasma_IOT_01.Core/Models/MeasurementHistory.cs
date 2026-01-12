using System;

namespace Flasma_IOT_01.Core.Models;

/// <summary>
/// Represents a measurement session history record
/// </summary>
public class MeasurementHistory
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Result { get; set; } = "OK"; // "OK" or "NG"
    public int TotalMeasurements { get; set; }
    public double AverageVoltage { get; set; }
    public double AverageCurrent { get; set; }
    public string FilePath { get; set; } = string.Empty;

    public TimeSpan Duration => EndTime - StartTime;

    public MeasurementHistory()
    {
    }

    public MeasurementHistory(DateTime startTime, DateTime endTime, string barcode, string result, 
        int totalMeasurements, double avgVoltage, double avgCurrent, string filePath)
    {
        StartTime = startTime;
        EndTime = endTime;
        Barcode = barcode;
        Result = result;
        TotalMeasurements = totalMeasurements;
        AverageVoltage = avgVoltage;
        AverageCurrent = avgCurrent;
        FilePath = filePath;
    }
}