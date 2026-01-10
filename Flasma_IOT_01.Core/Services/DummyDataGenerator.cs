using System;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// Generates dummy voltage and current data for testing purposes
/// </summary>
public class DummyDataGenerator
{
    private readonly Random _random = new();
    private double _baseVoltage = 220.0;
    private double _baseCurrent = 10.0;
    private int _sampleCount = 0;
    private DateTime _startTime = DateTime.UtcNow;

    /// <summary>
    /// Generate realistic voltage data with small variations
    /// </summary>
    public double GenerateVoltage()
    {
        var timeInSeconds = _sampleCount;
        
        // Simulate realistic AC voltage fluctuations
        // Main frequency: ~0.05 Hz (20 second cycle) for slow drift
        var slowDrift = Math.Sin(timeInSeconds * 0.05 * 2 * Math.PI) * 3; // ±3V slow drift
        
        // Medium frequency: ~0.2 Hz (5 second cycle) for load changes
        var loadChange = Math.Sin(timeInSeconds * 0.2 * 2 * Math.PI) * 2; // ±2V load variation
        
        // Random noise: typical measurement noise
        var noise = (_random.NextDouble() - 0.5) * 1.5; // ±0.75V noise
        
        // Occasional spike (1% chance)
        var spike = _random.NextDouble() < 0.01 ? (_random.NextDouble() - 0.5) * 10 : 0;
        
        return _baseVoltage + slowDrift + loadChange + noise + spike;
    }

    /// <summary>
    /// Generate realistic current data with small variations
    /// </summary>
    public double GenerateCurrent()
    {
        var timeInSeconds = _sampleCount;
        
        // Current typically has different pattern than voltage
        // Main frequency: ~0.08 Hz (12.5 second cycle)
        var slowDrift = Math.Cos(timeInSeconds * 0.08 * 2 * Math.PI) * 1.5; // ±1.5A slow drift
        
        // Medium frequency: ~0.15 Hz (6.67 second cycle) - slightly different from voltage
        var loadChange = Math.Sin(timeInSeconds * 0.15 * 2 * Math.PI) * 1; // ±1A load variation
        
        // Random noise: typical current measurement noise
        var noise = (_random.NextDouble() - 0.5) * 0.3; // ±0.15A noise
        
        // Occasional spike (1% chance) - smaller than voltage
        var spike = _random.NextDouble() < 0.01 ? (_random.NextDouble() - 0.5) * 2 : 0;
        
        // Current should never be negative
        var current = _baseCurrent + slowDrift + loadChange + noise + spike;
        return Math.Max(0, current);
    }

    /// <summary>
    /// Increment sample counter (call this every 1 second)
    /// </summary>
    public void IncrementSample()
    {
        _sampleCount++;
    }

    /// <summary>
    /// Reset the time counter for wave generation
    /// </summary>
    public void Reset()
    {
        _sampleCount = 0;
        _startTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Set custom base values for voltage and current
    /// </summary>
    public void SetBaseValues(double voltage, double current)
    {
        _baseVoltage = voltage;
        _baseCurrent = current;
    }

    /// <summary>
    /// Get current simulation time in seconds
    /// </summary>
    public double CurrentTime => _sampleCount;
}