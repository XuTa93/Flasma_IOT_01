using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;

namespace Flasma_IOT_01.Core.Controllers;

/// <summary>
/// Measurement controller that coordinates Start/Stop operations and events
/// </summary>
public class MeasurementController
{
    private readonly ModbusTcpClient _modbusClient;
    private readonly DataSampler _dataSampler;
    private readonly InMemoryDataRepository _dataRepository;
    private readonly ExcelReportExporter _reportExporter;
    private readonly DummyDataGenerator _dummyDataGenerator;
    private ModbusConnectionSettings? _modbusSettings;
    private CancellationTokenSource? _backgroundReadCts;
    private Task? _backgroundReadTask;
    private double _lastVoltage = 0;
    private double _lastCurrent = 0;
    private bool _isMeasuring = false;
    private bool _isDummyMode = false;
    private int _reconnectAttempts = 0;
    private bool _lastReportedConnectionState = false;
    private const int MaxReconnectAttempts = 10;
    private const int InitialRetryDelayMs = 1000;

    private DateTime _measurementStartTime;
    private readonly MeasurementHistoryRepository _historyRepository;

    public event EventHandler? MeasurementStarted;
    public event EventHandler? MeasurementStopped;
    public event EventHandler<string>? ErrorOccurred;
    public event EventHandler<NewDataReadEventArgs>? NewDataRead;
    public event EventHandler<bool>? ConnectionStatusChanged;

    public bool IsModbusConnected => _modbusClient.IsConnected;
    public bool IsDummyMode => _isDummyMode;

    public MeasurementController(
        ModbusTcpClient modbusClient,
        DataSampler dataSampler,
        InMemoryDataRepository dataRepository,
        ExcelReportExporter reportExporter,
        MeasurementHistoryRepository historyRepository)
    {
        _modbusClient = modbusClient ?? throw new ArgumentNullException(nameof(modbusClient));
        _dataSampler = dataSampler ?? throw new ArgumentNullException(nameof(dataSampler));
        _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        _reportExporter = reportExporter ?? throw new ArgumentNullException(nameof(reportExporter));
        _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
        _dummyDataGenerator = new DummyDataGenerator();

        _dataSampler.DataSampled += OnDataSampled;
    }

    /// <summary>
    /// Start dummy data mode for testing without Modbus connection
    /// </summary>
    public async Task StartDummyModeAsync(int samplingIntervalMs = 1000)
    {
        if (_backgroundReadTask != null && !_backgroundReadTask.IsCompleted)
            return;

        try
        {
            _isDummyMode = true;
            _dummyDataGenerator.Reset();
            _backgroundReadCts = new CancellationTokenSource();
            _reconnectAttempts = 0;

            _modbusSettings = new ModbusConnectionSettings
            {
                SamplingIntervalMs = samplingIntervalMs
            };

            _dataSampler.Start(samplingIntervalMs);
            NotifyConnectionStatusChangedIfNeeded(true); // Simulate connected state

            _backgroundReadTask = DummyDataLoopAsync(_backgroundReadCts.Token);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Dummy mode start failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Stop dummy data mode
    /// </summary>
    public async Task StopDummyModeAsync()
    {
        try
        {
            _dataSampler.Stop();
            _backgroundReadCts?.Cancel();
            if (_backgroundReadTask != null)
                await _backgroundReadTask;

            _isDummyMode = false;
            _reconnectAttempts = 0;
            NotifyConnectionStatusChangedIfNeeded(false);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Dummy mode stop failed: {ex.Message}");
            throw;
        }
        finally
        {
            _backgroundReadCts?.Dispose();
            _backgroundReadCts = null;
            _backgroundReadTask = null;
        }
    }

    /// <summary>
    /// Dummy data generation loop
    /// </summary>
    private async Task DummyDataLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);

                try
                {
                    _lastVoltage = _dummyDataGenerator.GenerateVoltage();
                    _lastCurrent = _dummyDataGenerator.GenerateCurrent();

                    // Invoke the NewDataRead event
                    NewDataRead?.Invoke(this, new NewDataReadEventArgs(_lastVoltage, _lastCurrent));
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Dummy data generation error: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Dummy mode fatal error: {ex.Message}");
        }
    }

    /// <summary>
    /// Start continuous background reading from Modbus
    /// </summary>
    public async Task StartBackgroundReadAsync(ModbusConnectionSettings modbusSettings)
    {
        if (_backgroundReadTask != null && !_backgroundReadTask.IsCompleted)
            return;

        try
        {
            _isDummyMode = false;
            _modbusSettings = modbusSettings ?? throw new ArgumentNullException(nameof(modbusSettings));
            _backgroundReadCts = new CancellationTokenSource();
            _reconnectAttempts = 0;

            await _modbusClient.ConnectAsync(modbusSettings.IpAddress, modbusSettings.Port);
            NotifyConnectionStatusChangedIfNeeded(_modbusClient.IsConnected);

            _dataSampler.Start(modbusSettings.SamplingIntervalMs);

            _backgroundReadTask = BackgroundReadLoopAsync(_backgroundReadCts.Token);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Background read failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Stop continuous background reading from Modbus
    /// </summary>
    public async Task StopBackgroundReadAsync()
    {
        try
        {
            _dataSampler.Stop();
            _backgroundReadCts?.Cancel();
            if (_backgroundReadTask != null)
                await _backgroundReadTask;

            await _modbusClient.DisconnectAsync();
            _reconnectAttempts = 0;
            NotifyConnectionStatusChangedIfNeeded(_modbusClient.IsConnected);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Background read stop failed: {ex.Message}");
            throw;
        }
        finally
        {
            _backgroundReadCts?.Dispose();
            _backgroundReadCts = null;
            _backgroundReadTask = null;
        }
    }

    /// <summary>
    /// Start collecting measurement data from pre-read buffer
    /// </summary>
    public async Task StartMeasurementAsync()
    {
        try
        {
            _measurementStartTime = DateTime.Now;
            _isMeasuring = true;
            MeasurementStarted?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Stop collecting measurement data
    /// </summary>
    public async Task StopMeasurementAsync()
    {
        try
        {
            _isMeasuring = false;
            MeasurementStopped?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Background loop that continuously reads from Modbus
    /// </summary>
    private async Task BackgroundReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);

                NotifyConnectionStatusChangedIfNeeded(_modbusClient.IsConnected);

                if (_modbusSettings == null)
                    continue;

                if (!_modbusClient.IsConnected)
                {
                    await AttemptReconnectAsync();
                    continue;
                }

                _reconnectAttempts = 0;

                try
                {
                    _lastVoltage = await ReadVoltageAsync(_modbusSettings);
                    _lastCurrent = await ReadCurrentAsync(_modbusSettings);

                    NewDataRead?.Invoke(this, new NewDataReadEventArgs(_lastVoltage, _lastCurrent));
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Background read error: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Background read fatal error: {ex.Message}");
        }
    }

    /// <summary>
    /// Called by DataSampler to collect current readings into repository
    /// </summary>
    private async void OnDataSampled(object? sender, EventArgs e)
    {
        try
        {
            if (!_isMeasuring)
                return;

            var measurement = new Measurement(_lastVoltage, _lastCurrent);
            _dataRepository.AddMeasurement(measurement);
            NewDataRead?.Invoke(this, new NewDataReadEventArgs(_lastVoltage, _lastCurrent));
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex.Message);
        }
    }

    private async Task AttemptReconnectAsync()
    {
        if (_modbusSettings == null || _reconnectAttempts >= MaxReconnectAttempts)
            return;

        try
        {
            _reconnectAttempts++;
            int delayMs = (int)(InitialRetryDelayMs * Math.Pow(2, _reconnectAttempts - 1));
            await Task.Delay(delayMs);

            await _modbusClient.ConnectAsync(_modbusSettings.IpAddress, _modbusSettings.Port);
            NotifyConnectionStatusChangedIfNeeded(_modbusClient.IsConnected);
            _reconnectAttempts = 0;
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Reconnection attempt {_reconnectAttempts} failed: {ex.Message}");
            NotifyConnectionStatusChangedIfNeeded(_modbusClient.IsConnected);
        }
    }

    private void NotifyConnectionStatusChangedIfNeeded(bool isConnected)
    {
        if (isConnected == _lastReportedConnectionState)
            return;

        _lastReportedConnectionState = isConnected;
        ConnectionStatusChanged?.Invoke(this, isConnected);
    }

    private async Task<double> ReadVoltageAsync(ModbusConnectionSettings settings)
    {
        var registers = await _modbusClient.ReadHoldingRegistersAsync(
            settings.VoltageRegisterAddress, 1);

        return registers.Length > 0 ? registers[0] : 0;
    }

    private async Task<double> ReadCurrentAsync(ModbusConnectionSettings settings)
    {
        var registers = await _modbusClient.ReadHoldingRegistersAsync(
            settings.CurrentRegisterAddress, 1);

        return registers.Length > 0 ? registers[0] : 0;
    }

    public IEnumerable<Measurement> GetAllMeasurements()
    {
        return _dataRepository.GetAllMeasurements();
    }

    public void ClearMeasurements()
    {
        _dataRepository.Clear();
    }

    public async Task ExportToExcelAsync(string filePath)
    {
        var measurements = _dataRepository.GetAllMeasurements();
        await _reportExporter.ExportToExcelAsync(filePath, measurements);
    }

    public async Task ExportToCsvAsync(string filePath)
    {
        var measurements = _dataRepository.GetAllMeasurements();
        await _reportExporter.ExportToCsvAsync(filePath, measurements);
    }

    /// <summary>
    /// Create a history record for the measurement
    /// </summary>
    public async Task<MeasurementHistory> CreateHistoryRecordAsync(string barcode, string csvFilePath)
    {
        var measurements = _dataRepository.GetAllMeasurements().ToList();
        var endTime = DateTime.Now;
        
        // Calculate statistics
        var avgVoltage = measurements.Any() ? measurements.Average(m => m.Voltage) : 0;
        var avgCurrent = measurements.Any() ? measurements.Average(m => m.Current) : 0;
        
        // TODO: Implement actual OK/NG logic based on chart analysis
        // For now, use random as placeholder
        var random = new Random();
        var result = random.Next(0, 2) == 0 ? "OK" : "NG";
        
        var history = new MeasurementHistory(
            _measurementStartTime,
            endTime,
            barcode,
            result,
            measurements.Count,
            avgVoltage,
            avgCurrent,
            csvFilePath
        );
        
        await _historyRepository.AddHistoryAsync(history);
        return history;
    }

    public IEnumerable<MeasurementHistory> GetAllHistory()
    {
        return _historyRepository.GetAllHistory();
    }
}

