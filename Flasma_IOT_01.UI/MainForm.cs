using Flasma_IOT_01.Core.Controllers;
using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;
using ScottPlot;

namespace Flasma_IOT_01.UI
{
    public partial class MainForm : Form
    {
        private readonly MeasurementController _measurementController;
        private readonly ModbusTcpClient _modbusClient;
        private readonly DataSampler _dataSampler;
        private readonly InMemoryDataRepository _dataRepository;
        private readonly ExcelReportExporter _reportExporter;

        private bool _isRecording = false;
        private int _lastMeasurementCount = 0;
        private DateTime _lastMeasurementTimestamp = DateTime.MinValue;

        public MainForm()
        {
            InitializeComponent();
            btnStart.Enabled = false;
            _modbusClient = new ModbusTcpClient();
            _dataSampler = new DataSampler();
            _reportExporter = new ExcelReportExporter();
            _dataRepository = new InMemoryDataRepository();
            _measurementController = new MeasurementController(
                _modbusClient,
                _dataSampler,
                _dataRepository,
                _reportExporter);

            // Subscribe to the NewDataRead event
            _measurementController.NewDataRead += OnMeasurementControllerNewDataRead;
            _measurementController.ErrorOccurred += OnMeasurementControllerErrorOccurred;
            _measurementController.MeasurementStarted += OnMeasurementControllerMeasurementStarted;
            _measurementController.MeasurementStopped += OnMeasurementControllerMeasurementStopped;
            _measurementController.ConnectionStatusChanged += _measurementController_ConnectionStatusChanged;

            // Initialize charts
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // Thiết lập Chart_Current
            Chart_Current.Plot.Title("Current (A)");
            Chart_Current.Plot.XLabel("Time (s)");
            Chart_Current.Plot.YLabel("Current (A)");
            Chart_Current.Plot.Axes.AutoScale();
            Chart_Current.Refresh();

            // Thiết lập Chart_Volt
            Chart_Volt.Plot.Title("Voltage (V)");
            Chart_Volt.Plot.XLabel("Time (s)");
            Chart_Volt.Plot.YLabel("Voltage (V)");
            Chart_Volt.Plot.Axes.AutoScale();
            Chart_Volt.Refresh();
        }

        private void chkDummyMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDummyMode.Checked)
            {
                // Enable dummy mode
                btnConnect.Enabled = false;
                btnStart.Enabled = true;
                _measurementController.StartDummyModeAsync(1000);
            }
            else
            {
                // Disable dummy mode
                _measurementController.StopDummyModeAsync();
                btnConnect.Enabled = !_measurementController.IsModbusConnected;
                btnStart.Enabled = _measurementController.IsModbusConnected;
            }
        }

        private void _measurementController_ConnectionStatusChanged(object? sender, bool e)
        {
            if (InvokeRequired)
            {
                Invoke(() => _measurementController_ConnectionStatusChanged(sender, e));
                return;
            }

            if (!chkDummyMode.Checked) // Only update if not in dummy mode
            {
                if (e == true)
                {
                    btnStart.Enabled = true;
                    btnConnect.Enabled = false;
                }
                else
                {
                    btnStart.Enabled = false;
                    btnConnect.Enabled = true;
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
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
            _measurementController.StartBackgroundReadAsync(settings);
        }

        private void BtnDisConnect_Click(object sender, EventArgs e)
        {
            if (_measurementController.IsDummyMode)
            {
                _measurementController.StopDummyModeAsync();
            }
            else
            {
                _measurementController.StopBackgroundReadAsync();
            }
        }

        private void OnMeasurementControllerNewDataRead(object? sender, NewDataReadEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMeasurementControllerNewDataRead(sender, e));
                return;
            }

            // Update UI labels
            lblCurentVolt.Text = $"Voltage: {e.Voltage:F2}V";
            lblCurrentAmpe.Text = $"Current: {e.Current:F2}A";

            // Chỉ vẽ chart khi đang recording (từ Start đến Stop)
            if (_isRecording)
            {
                UpdateChartsFromRepository();
            }
        }

        private void UpdateChartsFromRepository()
        {
            // Lấy tất cả dữ liệu từ repository
            var measurements = _measurementController.GetAllMeasurements().ToList();

            if (!measurements.Any())
                return;

            // Kiểm tra xem dữ liệu có thay đổi không
            var currentCount = measurements.Count;
            var lastTimestamp = measurements.Last().Timestamp;

            if (currentCount == _lastMeasurementCount && lastTimestamp == _lastMeasurementTimestamp)
            {
                // Dữ liệu không thay đổi, không cần vẽ lại
                return;
            }

            // Cập nhật cache
            _lastMeasurementCount = currentCount;
            _lastMeasurementTimestamp = lastTimestamp;

            // Chuẩn bị dữ liệu cho chart
            var voltages = new List<double>();
            var currents = new List<double>();
            var times = new List<double>();
            var startTime = measurements.First().Timestamp;

            foreach (var measurement in measurements)
            {
                voltages.Add(measurement.Voltage);
                currents.Add(measurement.Current);
                times.Add((measurement.Timestamp - startTime).TotalSeconds);
            }

            // Vẽ Chart_Volt
            Chart_Volt.Plot.Clear();
            var voltScatter = Chart_Volt.Plot.Add.Scatter(times.ToArray(), voltages.ToArray());
            voltScatter.LineWidth = 2;
            voltScatter.MarkerSize = 0;
            Chart_Volt.Plot.Axes.AutoScale();
            Chart_Volt.Refresh();

            // Vẽ Chart_Current
            Chart_Current.Plot.Clear();
            var currentScatter = Chart_Current.Plot.Add.Scatter(times.ToArray(), currents.ToArray());
            currentScatter.LineWidth = 2;
            currentScatter.MarkerSize = 0;
            Chart_Current.Plot.Axes.AutoScale();
            Chart_Current.Refresh();
        }

        private void OnMeasurementControllerErrorOccurred(object? sender, string e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMeasurementControllerErrorOccurred(sender, e));
                return;
            }

            MessageBox.Show($"Error: {e}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnMeasurementControllerMeasurementStarted(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMeasurementControllerMeasurementStarted(sender, e));
                return;
            }

            var mode = _measurementController.IsDummyMode ? "Dummy Mode" : "Real Data";
            MessageBox.Show($"Measurement Started ({mode})", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void OnMeasurementControllerMeasurementStopped(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMeasurementControllerMeasurementStopped(sender, e));
                return;
            }

            MessageBox.Show("Measurement Stopped", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Xuất CSV nếu có dữ liệu (gọi đến Core layer)
            await ExportDataToCsvAsync();
        }

        private async Task ExportDataToCsvAsync()
        {
            try
            {
                // Gọi MeasurementController để export CSV
                await _measurementController.ExportToCsvAsync(_reportExporter.GenerateDefaultFilePath());

                MessageBox.Show("Data exported successfully!",
                    "Export Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No measurements"))
            {
                // Không có dữ liệu, không hiển thị lỗi
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu cũ trong repository
            _measurementController.ClearMeasurements();

            // Reset cache
            _lastMeasurementCount = 0;
            _lastMeasurementTimestamp = DateTime.MinValue;

            // Bắt đầu recording
            _isRecording = true;

            // Xóa chart
            Chart_Current.Plot.Clear();
            Chart_Current.Refresh();

            Chart_Volt.Plot.Clear();
            Chart_Volt.Refresh();

            _measurementController.StartMeasurementAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Dừng recording
            _isRecording = false;
            _measurementController.StopMeasurementAsync();
        }

        /// <summary>
        /// Lấy dữ liệu đo lường từ kho dữ liệu và hiển thị trên giao diện người dùng
        /// </summary>
        private void btnTestData_Click(object sender, EventArgs e)
        {
            var data = _measurementController.GetAllMeasurements();
            MessageBox.Show($"Total measurements: {data.Count()}", "Data Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
