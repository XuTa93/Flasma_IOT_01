using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;

namespace Flasma_IOT_01.Core;

/// <summary>
/// Test program for Modbus TCP reading
/// </summary>
public class TestModbus
{
    public static async Task RunTestAsync()
    {
        Console.WriteLine("=== MODBUS TCP TEST PROGRAM ===\n");

        // T?o Modbus client
        using var modbusClient = new ModbusTcpClient();
        var measurements = new List<Measurement>();

        // C?u hình k?t n?i
        var ipAddress = "127.0.0.1";
        var port = 502;
        ushort voltageAddress = 0;
        ushort currentAddress = 1;
        
        Console.WriteLine($"Connecting to Modbus TCP Server: {ipAddress}:{port}");

        try
        {
            // K?t n?i
            await modbusClient.ConnectAsync(ipAddress, port);
            Console.WriteLine("? Connected successfully!\n");

            // Test 1: ??c 1 holding register (Voltage)
            Console.WriteLine("--- TEST 1: Read Single Holding Register (Voltage) ---");
            var voltageData = await modbusClient.ReadHoldingRegistersAsync(voltageAddress, 1);
            Console.WriteLine($"Register {voltageAddress}: {voltageData[0]} (Raw value)");
            Console.WriteLine($"Voltage: {voltageData[0] / 10.0:F2} V\n");

            // Test 2: ??c 1 holding register (Current)
            Console.WriteLine("--- TEST 2: Read Single Holding Register (Current) ---");
            var currentData = await modbusClient.ReadHoldingRegistersAsync(currentAddress, 1);
            Console.WriteLine($"Register {currentAddress}: {currentData[0]} (Raw value)");
            Console.WriteLine($"Current: {currentData[0] / 100.0:F2} A\n");

            // Test 3: ??c nhi?u registers cùng lúc
            Console.WriteLine("--- TEST 3: Read Multiple Holding Registers ---");
            var multipleData = await modbusClient.ReadHoldingRegistersAsync(voltageAddress, 2);
            Console.WriteLine($"Register {voltageAddress}: {multipleData[0]} (Voltage)");
            Console.WriteLine($"Register {currentAddress}: {multipleData[1]} (Current)");
            Console.WriteLine($"Voltage: {multipleData[0] / 10.0:F2} V");
            Console.WriteLine($"Current: {multipleData[1] / 100.0:F2} A\n");

            // Test 4: ??c liên t?c 5 l?n và l?u measurements
            Console.WriteLine("--- TEST 4: Continuous Reading (5 times) ---");
            for (int i = 1; i <= 5; i++)
            {
                var data = await modbusClient.ReadHoldingRegistersAsync(voltageAddress, 2);
                var voltage = data[0] / 10.0;
                var current = data[1] / 100.0;
                var power = voltage * current;

                measurements.Add(new Measurement(voltage, current));

                Console.WriteLine($"[{i}] Voltage: {voltage:F2} V | Current: {current:F2} A | Power: {power:F2} W");
                await Task.Delay(500); // ??i 500ms gi?a các l?n ??c
            }
            Console.WriteLine();

            // Test 5: Ghi register (n?u c?n)
            Console.WriteLine("--- TEST 5: Write Single Register ---");
            Console.WriteLine("Writing value 1234 to register 10...");
            await modbusClient.WriteRegisterAsync(10, 1234);
            Console.WriteLine("? Write successful!");
            
            // ??c l?i ?? verify
            var verifyData = await modbusClient.ReadHoldingRegistersAsync(10, 1);
            Console.WriteLine($"Verify read: {verifyData[0]}\n");

            // Test 6: Ghi nhi?u registers
            Console.WriteLine("--- TEST 6: Write Multiple Registers ---");
            ushort[] values = { 100, 200, 300, 400 };
            Console.WriteLine($"Writing values [{string.Join(", ", values)}] to registers 20-23...");
            await modbusClient.WriteMultipleRegistersAsync(20, values);
            Console.WriteLine("? Write successful!");
            
            // ??c l?i ?? verify
            var verifyMultiple = await modbusClient.ReadHoldingRegistersAsync(20, 4);
            Console.WriteLine($"Verify read: [{string.Join(", ", verifyMultiple)}]\n");

            // Test 7: ??c Input Registers
            Console.WriteLine("--- TEST 7: Read Input Registers ---");
            try
            {
                var inputData = await modbusClient.ReadInputRegistersAsync(0, 2);
                Console.WriteLine($"Input Register 0: {inputData[0]}");
                Console.WriteLine($"Input Register 1: {inputData[1]}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Input registers not available: {ex.Message}\n");
            }

            // Ng?t k?t n?i
            await modbusClient.DisconnectAsync();
            Console.WriteLine("? Disconnected successfully!");

            // Xu?t CSV
            await ExportMeasurementsAsync(measurements, "basic_test");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\n=== TEST COMPLETED ===");
    }

    /// <summary>
    /// Test v?i error handling và retry logic
    /// </summary>
    public static async Task RunAdvancedTestAsync()
    {
        Console.WriteLine("=== ADVANCED MODBUS TEST WITH ERROR HANDLING ===\n");

        using var modbusClient = new ModbusTcpClient();
        var measurements = new List<Measurement>();
        
        var settings = new ModbusConnectionSettings
        {
            IpAddress = "127.0.0.1",
            Port = 502,
            VoltageRegisterAddress = 0,
            CurrentRegisterAddress = 1,
            SamplingIntervalMs = 1000,
            TimeoutMs = 5000,
            RetryCount = 3
        };

        int successCount = 0;
        int errorCount = 0;

        try
        {
            Console.WriteLine($"Connecting to {settings.IpAddress}:{settings.Port}...");
            await modbusClient.ConnectAsync(settings.IpAddress, settings.Port);
            Console.WriteLine("? Connected!\n");

            Console.WriteLine("Starting continuous reading for 10 seconds...");
            Console.WriteLine("Time\t\tVoltage\t\tCurrent\t\tPower\t\tStatus");
            Console.WriteLine("????????????????????????????????????????????????????????????????");

            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                try
                {
                    var data = await modbusClient.ReadHoldingRegistersAsync(
                        settings.VoltageRegisterAddress, 2);
                    
                    var voltage = data[0] / 10.0;
                    var current = data[1] / 100.0;
                    var power = voltage * current;

                    measurements.Add(new Measurement(voltage, current));

                    Console.WriteLine(
                        $"{DateTime.Now:HH:mm:ss}\t{voltage:F2} V\t\t{current:F2} A\t\t{power:F2} W\t? OK");
                    
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"{DateTime.Now:HH:mm:ss}\t?\t\t?\t\t?\t\t? {ex.Message}");
                    errorCount++;

                    // Retry connection n?u m?t k?t n?i
                    if (!modbusClient.IsConnected)
                    {
                        Console.WriteLine("Attempting to reconnect...");
                        await Task.Delay(1000);
                        await modbusClient.ConnectAsync(settings.IpAddress, settings.Port);
                    }
                }

                await Task.Delay(settings.SamplingIntervalMs);
            }

            Console.WriteLine("????????????????????????????????????????????????????????????????");
            Console.WriteLine($"\nStatistics:");
            Console.WriteLine($"  Successful reads: {successCount}");
            Console.WriteLine($"  Failed reads: {errorCount}");
            Console.WriteLine($"  Success rate: {(successCount * 100.0 / (successCount + errorCount)):F2}%");

            await modbusClient.DisconnectAsync();

            // Xu?t CSV
            await ExportMeasurementsAsync(measurements, "advanced_test");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n? Fatal Error: {ex.Message}");
        }

        Console.WriteLine("\n=== ADVANCED TEST COMPLETED ===");
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
        Console.WriteLine($"? Exported to: {filePath}");

        // Show statistics
        Console.WriteLine();
        Console.WriteLine(exporter.GetStatisticsSummary(measurements));
    }
}
