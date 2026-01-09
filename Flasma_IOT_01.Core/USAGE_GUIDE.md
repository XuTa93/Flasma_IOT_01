# H??NG D?N S? D?NG - IoT Measurement System

## T?ng quan
H? th?ng ?o l??ng IoT v?i Modbus TCP và t? ??ng xu?t file CSV.

## Các ch? ?? test có s?n

### ?? Mode 1: Quick Read Test
**Mô t?:** Test nhanh, ??c 1 l?n duy nh?t
**Th?i gian:** ~1 giây
**Output:** 
- Console: K?t qu? ??c
- CSV: `quick_test_YYYYMMDD_HHMMSS.csv` (1 sample)

**S? d?ng khi:**
- Ki?m tra k?t n?i nhanh
- Verify Modbus server ho?t ??ng
- Test l?n ??u tiên

---

### ?? Mode 2: Continuous Read Test  
**Mô t?:** ??c liên t?c 10 l?n, m?i l?n cách nhau 1 giây
**Th?i gian:** ~10 giây
**Output:**
- Console: B?ng d? li?u real-time
- CSV: `continuous_test_YYYYMMDD_HHMMSS.csv` (10 samples)
- Statistics: Min/Max/Avg c?a Voltage, Current, Power

**S? d?ng khi:**
- Ki?m tra tính ?n ??nh
- Thu th?p d? li?u m?u
- ?ánh giá ch?t l??ng tín hi?u

---

### ?? Mode 3: Write & Verify Test
**Mô t?:** Test ghi d? li?u vào registers
**Th?i gian:** ~2 giây
**Output:**
- Console: K?t qu? ghi và verify
- CSV: Không có

**Tests:**
1. Write single register
2. Write multiple registers
3. Read back ?? verify

**S? d?ng khi:**
- C?n test write operations
- Verify kh? n?ng ghi c?a Modbus server

---

### ?? Mode 4: Interactive Test
**Mô t?:** Test v?i parameters tùy ch?nh
**Th?i gian:** Tùy ch?n
**Input:**
- IP Address
- Port
- Start Register Address
- Number of Registers
- Number of Reads

**Output:**
- Console: D? li?u ??c ???c
- CSV: `interactive_test_YYYYMMDD_HHMMSS.csv` (n?u ??c voltage/current)

**S? d?ng khi:**
- C?n test v?i custom configuration
- ??c các registers không ph?i voltage/current
- Debug specific addresses

---

### ?? Mode 5: Basic Modbus Test
**Mô t?:** Test ??y ?? các ch?c n?ng Modbus
**Th?i gian:** ~5 giây
**Output:**
- CSV: `basic_test_YYYYMMDD_HHMMSS.csv` (5 samples)

**Tests bao g?m:**
1. Read single holding register (Voltage)
2. Read single holding register (Current)
3. Read multiple holding registers
4. Continuous reading (5 times) ? **L?u vào CSV**
5. Write single register
6. Write multiple registers
7. Read input registers

**S? d?ng khi:**
- Test t?ng th? t?t c? functions
- ?ánh giá kh? n?ng c?a Modbus server

---

### ??? Mode 6: Advanced Modbus Test
**Mô t?:** Test v?i error handling và retry logic
**Th?i gian:** 10 giây
**Output:**
- Console: Real-time monitoring v?i status
- CSV: `advanced_test_YYYYMMDD_HHMMSS.csv` (~10 samples)
- Statistics: Success rate, Error count

**Features:**
- Auto reconnect khi m?t k?t n?i
- Error tracking
- Performance statistics

**S? d?ng khi:**
- Ki?m tra ?? tin c?y
- Test trong môi tr??ng không ?n ??nh
- ?ánh giá success rate

---

### ?? Mode 7: Full System Test
**Mô t?:** Test toàn b? h? th?ng v?i Controller integration
**Th?i gian:** ~12 giây
**Output:**
- CSV: `full_system_test_YYYYMMDD_HHMMSS.csv` (~10 samples)
- Statistics: T? InMemoryDataRepository

**Components tested:**
- ModbusTcpClient
- DataSampler
- InMemoryDataRepository
- MeasurementController
- Background reading
- Event handling

**S? d?ng khi:**
- Test integration
- Ki?m tra toàn b? workflow
- Production-like testing

---

### ?? Mode 8: CSV Export Examples
**Mô t?:** Demo các tính n?ng c?a CsvExporter (không c?n Modbus)
**Th?i gian:** ~5 giây
**Output:** Nhi?u file CSV m?u

**Examples:**
1. Basic export
2. Custom format (delimiter, date format)
3. Export to string
4. Batch export
5. Large dataset (1000 samples)
6. Statistics only

**S? d?ng khi:**
- H?c cách dùng CsvExporter
- Test CSV export mà không c?n Modbus server
- Demo cho ng??i khác

---

## File CSV Output

### V? trí
```
Flasma_IOT_01.Core\bin\Debug\net8.0\
```

### Format
```csv
Id,Timestamp,Voltage (V),Current (A),Power (W)
1,2024-01-15 14:30:25.123,220.00,1.500,330.00
2,2024-01-15 14:30:26.125,220.10,1.505,331.25
```

### M? file CSV
- **Excel:** Double-click ho?c File > Open
- **Google Sheets:** File > Import > Upload
- **Python:** `pd.read_csv('filename.csv')`
- **Text Editor:** Notepad++, VS Code

---

## Requirements

### Modbus Server
- **IP:** 127.0.0.1 (default)
- **Port:** 502 (default)
- **Registers:**
  - 0: Voltage (raw value, chia 10 ?? ra Volt)
  - 1: Current (raw value, chia 100 ?? ra Ampere)

### Software ?? mô ph?ng
- ModRSsim2 (Windows, Free)
- Modbus Slave (Windows, Commercial)
- pyModSlave (Python, Free)
- diagslave (Cross-platform, Free)

---

## Ví d? s? d?ng

### Scenario 1: L?n ??u test
```
Ch?n: Mode 1 (Quick Read Test)
M?c ?ích: Ki?m tra k?t n?i
K?t qu?: 1 file CSV v?i 1 sample
```

### Scenario 2: Thu th?p d? li?u m?u
```
Ch?n: Mode 2 (Continuous Read Test)
M?c ?ích: L?y 10 samples ?? phân tích
K?t qu?: 1 file CSV v?i 10 samples + statistics
```

### Scenario 3: Test production-like
```
Ch?n: Mode 7 (Full System Test)
M?c ?ích: Ki?m tra toàn b? h? th?ng
K?t qu?: 1 file CSV + event logs + statistics
```

### Scenario 4: Custom configuration
```
Ch?n: Mode 4 (Interactive Test)
Nh?p: IP=192.168.1.100, Port=502, Address=10, Count=5, Reads=20
K?t qu?: 1 file CSV v?i custom data
```

---

## Statistics Summary

T?t c? các mode (tr? mode 3 và 8) ??u hi?n th? statistics:

```
=== MEASUREMENT STATISTICS ===
Total Samples: 10
Time Range: 2024-01-15 14:30:25 to 2024-01-15 14:30:35
Duration: 10.00 seconds

Voltage:
  Min: 219.80 V
  Max: 220.20 V
  Avg: 220.00 V

Current:
  Min: 1.498 A
  Max: 1.502 A
  Avg: 1.500 A

Power:
  Min: 329.50 W
  Max: 330.50 W
  Avg: 330.00 W
```

---

## Troubleshooting

### "No connection could be made"
? Modbus server không ch?y ho?c sai IP/Port

### "Connection timed out"
? Firewall block port 502 ho?c network issue

### "Slave device failed to respond"
? Sai register address ho?c slave ID

### File CSV không ???c t?o
? Không có quy?n ghi file ho?c test không có measurements

### D? li?u sai
? Ki?m tra conversion factors (÷10 cho voltage, ÷100 cho current)

---

## Best Practices

1. **Test l?n ??u:** Dùng Mode 1 ?? verify connection
2. **Thu th?p data:** Dùng Mode 2 ho?c Mode 6
3. **Production test:** Dùng Mode 7
4. **Debug:** Dùng Mode 4 v?i custom parameters
5. **Learning:** Dùng Mode 8 ?? h?c CSV export

---

## Quick Start

```bash
# 1. Build project
dotnet build

# 2. Run
dotnet run

# 3. Ch?n mode t? menu (1-8)

# 4. Check output folder cho CSV files
cd bin\Debug\net8.0\
dir *.csv
```

---

## Support Files

- `MODBUS_TEST_README.md` - Chi ti?t v? Modbus testing
- `CSV_EXPORT_README.md` - Chi ti?t v? CSV export
- `CsvExporterExamples.cs` - Code examples

---

## Contact

Project: EWI-TEMAS FLASMA IoT Measurement System
