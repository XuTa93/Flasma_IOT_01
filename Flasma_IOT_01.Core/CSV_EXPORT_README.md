# CSV Export Feature

T?t c? các ch? ?? test ??u t? ??ng xu?t file CSV sau khi hoàn thành.

## File CSV ???c t?o

### Tên file
File CSV ???c t? ??ng ??t tên theo format:
```
{test_name}_{timestamp}.csv
```

Ví d?:
- `quick_test_20240115_143025.csv`
- `continuous_test_20240115_143530.csv`
- `basic_test_20240115_144012.csv`
- `advanced_test_20240115_144520.csv`
- `full_system_test_20240115_145030.csv`

### V? trí l?u file
File CSV ???c l?u trong th? m?c hi?n t?i (working directory), th??ng là:
```
Flasma_IOT_01.Core\bin\Debug\net8.0\
```

## C?u trúc file CSV

### Format chu?n
```csv
Id,Timestamp,Voltage (V),Current (A),Power (W)
1,2024-01-15 14:30:25.123,220.00,1.500,330.00
2,2024-01-15 14:30:26.125,220.10,1.505,331.25
3,2024-01-15 14:30:27.127,220.05,1.502,330.48
```

### Các c?t d? li?u

| C?t | Mô t? | ??n v? | Format |
|-----|-------|--------|--------|
| Id | ID c?a measurement | - | Integer |
| Timestamp | Th?i ?i?m ?o | - | yyyy-MM-dd HH:mm:ss.fff |
| Voltage (V) | ?i?n áp | Volt | F2 (2 decimal places) |
| Current (A) | Dòng ?i?n | Ampere | F3 (3 decimal places) |
| Power (W) | Công su?t | Watt | F2 (2 decimal places) |

## Statistics Summary

Sau khi xu?t CSV, h? th?ng t? ??ng hi?n th? th?ng kê:

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

## S? d?ng CSV trong các test

### Test 1: Quick Read Test
- **File:** `quick_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** 1 measurement
- **Mô t?:** Ch? có 1 dòng d? li?u t? l?n ??c duy nh?t

### Test 2: Continuous Read Test
- **File:** `continuous_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** 10 measurements (default)
- **Mô t?:** D? li?u ??c liên t?c v?i interval 1 giây

### Test 3: Write & Verify Test
- **File:** Không xu?t CSV (test ch? ghi, không thu th?p measurements)

### Test 4: Interactive Test
- **File:** `interactive_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** Tùy theo s? l?n ??c do user nh?p
- **?i?u ki?n:** Ch? xu?t n?u ??c voltage và current t? register 0-1

### Test 5: Basic Modbus Test
- **File:** `basic_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** 5 measurements
- **Mô t?:** T? Test 4 (Continuous Reading - 5 times)

### Test 6: Advanced Modbus Test
- **File:** `advanced_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** ~10 measurements (tùy vào success rate)
- **Mô t?:** ??c liên t?c trong 10 giây, có error handling

### Test 7: Full System Test
- **File:** `full_system_test_YYYYMMDD_HHMMSS.csv`
- **Samples:** ~10 measurements
- **Mô t?:** D? li?u t? InMemoryDataRepository, background reading

## M? file CSV

### Microsoft Excel
```
File > Open > Ch?n file CSV
```
Ho?c double-click vào file CSV

### Google Sheets
```
File > Import > Upload > Ch?n file CSV
```

### Python (Pandas)
```python
import pandas as pd
df = pd.read_csv('measurements_20240115_143025.csv')
print(df.head())
```

### R
```r
data <- read.csv('measurements_20240115_143025.csv')
head(data)
```

### Power BI
```
Get Data > Text/CSV > Ch?n file
```

## Custom Export Options

### CsvExporter API

```csharp
var exporter = new CsvExporter();

// Export c? b?n
await exporter.ExportToFileAsync(measurements, "output.csv");

// Export không có header
await exporter.ExportToFileAsync(measurements, "output.csv", includeHeader: false);

// Export không có Power column
await exporter.ExportToFileAsync(measurements, "output.csv", includePower: false);

// Export custom format v?i delimiter
await exporter.ExportCustomFormatAsync(
    measurements, 
    "output.csv", 
    delimiter: ";", 
    dateFormat: "dd/MM/yyyy HH:mm:ss");

// Export to string
string csvContent = exporter.ExportToString(measurements);

// Generate filename v?i timestamp
string fileName = exporter.GenerateFileName("my_test");
// Result: my_test_20240115_143025.csv

// Get statistics
string stats = exporter.GetStatisticsSummary(measurements);
Console.WriteLine(stats);
```

## Ví d? Output

### Console Output sau khi test
```
=== CONTINUOUS MODBUS READ TEST ===
Server: 127.0.0.1:502
Reads: 10 times
Interval: 1000ms

? Connected

Time            Voltage         Current         Power           Raw Values
?????????????????????????????????????????????????????????????????????
14:30:25.123    220.00 V        1.500 A         330.00 W        [2200, 150]
14:30:26.125    220.10 V        1.505 A         331.25 W        [2201, 151]
...
?????????????????????????????????????????????????????????????????????

? Disconnected

Exporting 10 measurement(s) to CSV...
? Exported to: D:\...\continuous_test_20240115_143026.csv

=== MEASUREMENT STATISTICS ===
Total Samples: 10
Time Range: 2024-01-15 14:30:25 to 2024-01-15 14:30:35
Duration: 10.00 seconds

Voltage:
  Min: 220.00 V
  Max: 220.10 V
  Avg: 220.05 V

Current:
  Min: 1.500 A
  Max: 1.505 A
  Avg: 1.502 A

Power:
  Min: 330.00 W
  Max: 331.25 W
  Avg: 330.62 W

? Test Completed
```

## L?u ý

1. **File encoding:** UTF-8
2. **Decimal separator:** D?u ch?m (.) - theo InvariantCulture
3. **Date format:** ISO 8601 v?i milliseconds
4. **Line ending:** Windows style (CRLF)
5. **Overwrite:** File CSV m?i ???c t?o v?i timestamp unique, không overwrite

## Troubleshooting

### File không ???c t?o
- Ki?m tra quy?n ghi trong th? m?c
- Ki?m tra console có thông báo l?i không
- Verify test có measurements không (minimum 1)

### File b? l?i encoding
- File s? d?ng UTF-8, m? b?ng Notepad++ ho?c VS Code ?? ki?m tra
- Excel có th? hi?n th? sai v?i m?t s? ký t? ??c bi?t

### S? li?u không chính xác
- Ki?m tra raw values trong console output
- Verify conversion factors (Voltage ÷ 10, Current ÷ 100)
- Ki?m tra Modbus server có tr? v? ?úng d? li?u không
