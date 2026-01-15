namespace Flasma_IOT_01.Core.Models;

/// <summary>
/// Modbus TCP connection settings
/// </summary>
public class ModbusConnectionSettings
{
    /// <summary>
    /// Sampling frequency in milliseconds
    /// </summary>
    public int SamplingIntervalMs { get; set; } = 1000;

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int TimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Modbus server IP address
    /// </summary>
    public string IpAddress { get; set; } = "127.0.0.1";

    /// <summary>
    /// Modbus server port
    /// </summary>
    public int Port { get; set; } = 502;

    /// <summary>
    /// Starting register address for voltage
    /// </summary>
    public ushort VoltageRegisterAddress { get; set; } = 0;

    /// <summary>
    /// Starting register address for current
    /// </summary>
    public ushort CurrentRegisterAddress { get; set; } = 1;

    /// <summary>
    /// Starting register address for Alarm
    /// </summary>
    public ushort AlarmRegisterAddress { get; set; } = 2;

    /// <summary>
    /// Starting register address for power
    /// </summary>
    public ushort PowerRegisterAddress { get; set; } = 3;

    public ushort CoilDoorCloseRegisterAddress { get; set; } = 0;
    public ushort CoilDoorOpenRegisterAddress { get; set; } = 1;
    public ushort CoilReadyRegisterAddress { get; set; } = 2;
    public ushort CoilRunningRegisterAddress { get; set; } = 3;
    public ushort CoilStartRegisterAddress { get; set; } = 4;
    public ushort CoilStopRegisterAddress { get; set; } = 5;

}
