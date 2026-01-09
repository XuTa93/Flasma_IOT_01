using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;

namespace Flasma_IOT_01.Core;

/// <summary>
/// Simple quick test for Modbus reading
/// </summary>
public class QuickModbusTest
{
    /// <summary>
    /// Chạy test đơn giản chỉ đọc một lần
    /// </summary>
    public static async Task RunQuickReadTestAsync(string ipAddress = "127.0.0.1", int port = 502)
    {
        Console.WriteLine($"\n=== QUICK MODBUS READ TEST ===");
        Console.WriteLine($"Server: {ipAddress}:{port}\n");

        using var client = new ModbusTcpClient();
        var measurements = new List<Measurement>();

        try
        {
            // Kết nối
            Console.Write("Connecting... ");
            await client.ConnectAsync(ipAddress, port);
            Console.WriteLine("✓ Connected");

            // Đọc 2 registers
            Console.Write("Reading registers 0-1... ");
            var data = await client.ReadHoldingRegistersAsync(0, 2);
            Console.WriteLine("✓ Success");

            // Lưu measurement
            var voltage = data[0] / 10.0;
            var current = data[1] / 100.0;
            measurements.Add(new Measurement(voltage, current));

            // Hiển thị kết quả
            Console.WriteLine("\n--- Results ---");
            Console.WriteLine($"Register 0 (Voltage): {data[0]} → {voltage:F2} V");
            Console.WriteLine($"Register 1 (Current): {data[1]} → {current:F2} A");
            Console.WriteLine($"Calculated Power: {voltage * current:F2} W");

            // Ngắt kết nối
            Console.Write("\nDisconnecting... ");
            await client.DisconnectAsync();
            Console.WriteLine("✓ Disconnected");

            // Xuất CSV
            await ExportMeasurementsAsync(measurements, "quick_test");

            Console.WriteLine("\n✓ Test PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Test FAILED");
            Console.WriteLine($"Error: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// Test đọc liên tục với số lần chỉ định
    /// </summary>
    public static async Task RunContinuousTestAsync(
        string ipAddress = "127.0.0.1", 
        int port = 502, 
        int readCount = 10, 
        int intervalMs = 1000)
    {
        Console.WriteLine($"\n=== CONTINUOUS MODBUS READ TEST ===");
        Console.WriteLine($"Server: {ipAddress}:{port}");
        Console.WriteLine($"Reads: {readCount} times");
        Console.WriteLine($"Interval: {intervalMs}ms\n");

        using var client = new ModbusTcpClient();
        var measurements = new List<Measurement>();

        try
        {
            await client.ConnectAsync(ipAddress, port);
            Console.WriteLine("✓ Connected\n");

            Console.WriteLine("Time\t\tVoltage\t\tCurrent\t\tPower\t\tRaw Values");
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");

            for (int i = 1; i <= readCount; i++)
            {
                try
                {
                    var data = await client.ReadHoldingRegistersAsync(0, 2);
                    var voltage = data[0] / 10.0;
                    var current = data[1] / 100.0;
                    var power = voltage * current;

                    // Lưu measurement
                    measurements.Add(new Measurement(voltage, current));

                    Console.WriteLine(
                        $"{DateTime.Now:HH:mm:ss.fff}\t" +
                        $"{voltage,7:F2} V\t" +
                        $"{current,7:F3} A\t" +
                        $"{power,7:F2} W\t" +
                        $"[{data[0]}, {data[1]}]");

                    if (i < readCount)
                        await Task.Delay(intervalMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\t✗ Error: {ex.Message}");
                }
            }

            Console.WriteLine("─────────────────────────────────────────────────────────────────────");
            await client.DisconnectAsync();

            // Xuất CSV và statistics
            await ExportMeasurementsAsync(measurements, "continuous_test");

            Console.WriteLine("\n✓ Test Completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Connection Failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Test ghi và đọc lại để verify
    /// </summary>
    public static async Task RunWriteTestAsync(
        string ipAddress = "127.0.0.1", 
        int port = 502)
    {
        Console.WriteLine($"\n=== MODBUS WRITE TEST ===");
        Console.WriteLine($"Server: {ipAddress}:{port}\n");

        using var client = new ModbusTcpClient();

        try
        {
            await client.ConnectAsync(ipAddress, port);
            Console.WriteLine("✓ Connected\n");

            // Test 1: Write single register
            Console.WriteLine("Test 1: Write Single Register");
            ushort testAddress = 10;
            ushort testValue = 12345;

            Console.WriteLine($"  Writing value {testValue} to register {testAddress}...");
            await client.WriteRegisterAsync(testAddress, testValue);
            Console.WriteLine("  ✓ Write completed");

            Console.WriteLine($"  Reading back register {testAddress}...");
            var readValue = await client.ReadHoldingRegistersAsync(testAddress, 1);
            Console.WriteLine($"  Read value: {readValue[0]}");

            if (readValue[0] == testValue)
                Console.WriteLine("  ✓ Verification PASSED\n");
            else
                Console.WriteLine($"  ✗ Verification FAILED (expected {testValue}, got {readValue[0]})\n");

            // Test 2: Write multiple registers
            Console.WriteLine("Test 2: Write Multiple Registers");
            ushort startAddress = 10;
            ushort[] testValues = { 100, 200, 300, 400, 500 };

            Console.WriteLine($"  Writing values [{string.Join(", ", testValues)}] to registers {startAddress}-{startAddress + testValues.Length - 1}...");
            await client.WriteMultipleRegistersAsync(startAddress, testValues);
            Console.WriteLine("  ✓ Write completed");

            Console.WriteLine($"  Reading back registers {startAddress}-{startAddress + testValues.Length - 1}...");
            var readValues = await client.ReadHoldingRegistersAsync(startAddress, (ushort)testValues.Length);
            Console.WriteLine($"  Read values: [{string.Join(", ", readValues)}]");

            bool allMatch = true;
            for (int i = 0; i < testValues.Length; i++)
            {
                if (testValues[i] != readValues[i])
                {
                    allMatch = false;
                    Console.WriteLine($"  ✗ Mismatch at index {i}: expected {testValues[i]}, got {readValues[i]}");
                }
            }

            if (allMatch)
                Console.WriteLine("  ✓ Verification PASSED\n");
            else
                Console.WriteLine("  ✗ Verification FAILED\n");

            await client.DisconnectAsync();
            Console.WriteLine("✓ Test Completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Test Failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper method to export measurements to CSV
    /// </summary>
    private static async Task ExportMeasurementsAsync(List<Measurement> measurements, string prefix)
    {
        if (!measurements.Any())
        {
            Console.WriteLine("\nNo measurements to export.");
            return;
        }

        var exporter = new CsvExporter();
        var fileName = exporter.GenerateFileName(prefix);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        Console.WriteLine($"\nExporting {measurements.Count} measurement(s) to CSV...");
        await exporter.ExportToFileAsync(measurements, filePath);
        Console.WriteLine($"✓ Exported to: {filePath}");

        // Show statistics
        Console.WriteLine();
        Console.WriteLine(exporter.GetStatisticsSummary(measurements));
    }
}
