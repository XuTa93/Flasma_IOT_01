using Flasma_IOT_01.Core;
using Flasma_IOT_01.Core.Controllers;
using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;
using Flasma_IOT_01.Core.Examples;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║    IoT Measurement System - Modbus TCP Test Suite         ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Select test mode:");
Console.WriteLine("1. Quick Read Test (Single read)");
Console.WriteLine("2. Continuous Read Test (10 reads)");
Console.WriteLine("3. Write & Verify Test");
Console.WriteLine("4. Basic Modbus Test (All features)");
Console.WriteLine("5. Advanced Modbus Test (Error handling)");
Console.WriteLine("6. CSV Export Examples (Demo only, no Modbus)");
Console.WriteLine();
Console.Write("Enter your choice (1-7): ");

var choice = Console.ReadLine();

try
{
    switch (choice)
    {
        case "1":
            await QuickModbusTest.RunQuickReadTestAsync();
            break;

        case "2":
            await QuickModbusTest.RunContinuousTestAsync(
                readCount: 10, 
                intervalMs: 1000);
            break;

        case "3":
            await QuickModbusTest.RunWriteTestAsync();
            break;

        case "4":
            await TestModbus.RunTestAsync();
            break;

        case "5":
            await TestModbus.RunAdvancedTestAsync();
            break;

        case "6":
            await CsvExporterExamples.RunAllExamples();
            break;

        default:
            Console.WriteLine("\nInvalid choice. Running quick test...");
            await QuickModbusTest.RunQuickReadTestAsync();
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ Unhandled Exception: {ex.Message}");
    Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
}

Console.WriteLine("\n\nPress any key to exit...");
Console.ReadKey();