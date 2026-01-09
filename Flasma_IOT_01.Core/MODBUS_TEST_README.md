# Modbus TCP Test Suite

H? th?ng test ??y ?? cho vi?c ??c/ghi d? li?u Modbus TCP.

## Các ch? ?? test

### 1. Quick Read Test (Test nhanh)
- K?t n?i ??n Modbus server
- ??c 2 registers (Voltage và Current)
- Hi?n th? k?t qu? và ng?t k?t n?i
- **Th?i gian:** ~1 giây
- **Phù h?p cho:** Ki?m tra k?t n?i nhanh

### 2. Continuous Read Test (??c liên t?c)
- ??c d? li?u liên t?c 10 l?n
- Kho?ng cách m?i l?n ??c: 1 giây
- Hi?n th? b?ng d? li?u v?i timestamp
- **Th?i gian:** ~10 giây
- **Phù h?p cho:** Ki?m tra tính ?n ??nh c?a k?t n?i

### 3. Write & Verify Test (Test ghi d? li?u)
- Test ghi single register
- Test ghi multiple registers
- T? ??ng verify d? li?u sau khi ghi
- **Phù h?p cho:** Ki?m tra ch?c n?ng ghi d? li?u

### 4. Interactive Test (Test t??ng tác)
- Nh?p thông s? tùy ch?nh (IP, Port, Address, Count)
- Linh ho?t cho m?i c?u hình Modbus
- **Phù h?p cho:** Test v?i c?u hình ??c bi?t

### 5. Basic Modbus Test (Test c? b?n ??y ??)
- 7 test cases khác nhau
- Bao g?m: read single, read multiple, write single, write multiple, read input registers
- Hi?n th? chi ti?t t?ng b??c
- **Th?i gian:** ~5 giây
- **Phù h?p cho:** Ki?m tra toàn b? tính n?ng Modbus

### 6. Advanced Modbus Test (Test nâng cao)
- Test v?i error handling
- Retry logic khi m?t k?t n?i
- Statistics (success rate, error count)
- ??c liên t?c trong 10 giây
- **Phù h?p cho:** Ki?m tra ?? tin c?y

### 7. Full System Test (Test toàn h? th?ng)
- Test integration v?i MeasurementController
- Background reading
- Data sampling và storage
- Event handling
- **Th?i gian:** ~12 giây
- **Phù h?p cho:** Ki?m tra toàn b? h? th?ng

## Cách ch?y

### T? Visual Studio
1. M? solution
2. Set `Flasma_IOT_01.Core` làm startup project
3. Nh?n F5 ho?c Ctrl+F5
4. Ch?n ch? ?? test t? menu

### T? Command Line
```bash
cd Flasma_IOT_01.Core
dotnet run
```

### T? file EXE
```bash
cd Flasma_IOT_01.Core\bin\Debug\net8.0
Flasma_IOT_01.Core.exe
```

## Yêu c?u h? th?ng

### Modbus Server
- IP Address: 127.0.0.1 (m?c ??nh)
- Port: 502 (m?c ??nh)
- Registers:
  - Address 0: Voltage (ví d?: 2200 = 220.0V)
  - Address 1: Current (ví d?: 150 = 1.50A)

### Ph?n m?m mô ph?ng Modbus Server (n?u c?n test)
- **ModRSsim2** (Windows) - Free
- **Modbus Slave** (Windows) - Commercial
- **pyModSlave** (Cross-platform) - Free
- **diagslave** (Cross-platform) - Free

## C?u hình m?c ??nh

```csharp
IP Address: 127.0.0.1
Port: 502
Voltage Register: 0
Current Register: 1
Sampling Interval: 1000ms
Timeout: 5000ms
Retry Count: 3
```

## Cách ??c k?t qu?

### Voltage
- Raw value ???c chia cho 10
- Ví d?: 2200 ? 220.0V

### Current
- Raw value ???c chia cho 100
- Ví d?: 150 ? 1.50A

### Power
- ???c tính b?ng: Voltage × Current
- Ví d?: 220V × 1.5A = 330W

## Troubleshooting

### L?i: "No connection could be made"
- Ki?m tra Modbus server có ?ang ch?y không
- Ki?m tra IP và Port có ?úng không
- Ki?m tra firewall có block port 502 không

### L?i: "Connection timed out"
- T?ng timeout trong ModbusConnectionSettings
- Ki?m tra network connection

### L?i: "Slave device failed to respond"
- Ki?m tra slave ID (m?c ??nh: 1)
- Ki?m tra register address có ?úng không
- Ki?m tra Modbus server có h? tr? function code không

## API Reference

### ModbusTcpClient

```csharp
// K?t n?i
await modbusClient.ConnectAsync(ipAddress, port);

// ??c holding registers
ushort[] data = await modbusClient.ReadHoldingRegistersAsync(startAddress, quantity);

// ??c input registers
ushort[] data = await modbusClient.ReadInputRegistersAsync(startAddress, quantity);

// Ghi single register
await modbusClient.WriteRegisterAsync(address, value);

// Ghi multiple registers
await modbusClient.WriteMultipleRegistersAsync(startAddress, values);

// Ng?t k?t n?i
await modbusClient.DisconnectAsync();
```

### QuickModbusTest

```csharp
// Quick read test
await QuickModbusTest.RunQuickReadTestAsync("127.0.0.1", 502);

// Continuous test
await QuickModbusTest.RunContinuousTestAsync("127.0.0.1", 502, readCount: 10, intervalMs: 1000);

// Write test
await QuickModbusTest.RunWriteTestAsync("127.0.0.1", 502);

// Interactive test
await QuickModbusTest.RunInteractiveTestAsync();
```

## Ví d? Output

### Quick Read Test
```
=== QUICK MODBUS READ TEST ===
Server: 127.0.0.1:502

Connecting... ? Connected
Reading registers 0-1... ? Success

--- Results ---
Register 0 (Voltage): 2200 ? 220.00 V
Register 1 (Current): 150 ? 1.50 A
Calculated Power: 330.00 W

Disconnecting... ? Disconnected

? Test PASSED
```

### Continuous Read Test
```
Time            Voltage         Current         Power           Raw Values
?????????????????????????????????????????????????????????????????????
14:30:01.123    220.00 V        1.500 A         330.00 W        [2200, 150]
14:30:02.125    220.10 V        1.505 A         331.25 W        [2201, 151]
14:30:03.127    220.05 V        1.502 A         330.48 W        [2201, 150]
...
```

## Dependencies

- **.NET 8.0**
- **NModbus4** (2.0.2) - Modbus protocol implementation
- **System.IO.Ports** (8.0.0) - Required by NModbus4

## License

Internal use only - EWI-TEMAS FLASMA Project
