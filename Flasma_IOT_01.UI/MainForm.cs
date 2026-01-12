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
        private readonly MeasurementHistoryRepository _historyRepository;

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
            _historyRepository = new MeasurementHistoryRepository();
            _measurementController = new MeasurementController(
                _modbusClient,
                _dataSampler,
                _dataRepository,
                _reportExporter,
                _historyRepository);

            // Subscribe to the NewDataRead event
            _measurementController.NewDataRead += OnMeasurementControllerNewDataRead;
            _measurementController.ErrorOccurred += OnMeasurementControllerErrorOccurred;
            _measurementController.MeasurementStarted += OnMeasurementControllerMeasurementStarted;
            _measurementController.MeasurementStopped += OnMeasurementControllerMeasurementStopped;
            _measurementController.ConnectionStatusChanged += _measurementController_ConnectionStatusChanged;

            // Initialize charts
            InitializeCharts();
            
            // Initialize history grid
            InitializeHistoryGrid();
            chkDummyMode.Checked = true;
        }

        private void InitializeHistoryGrid()
        {
            dgvHistory.Columns.Add("Id", "ID");
            dgvHistory.Columns.Add("StartTime", "Start Time");
            dgvHistory.Columns.Add("EndTime", "End Time");
            dgvHistory.Columns.Add("Duration", "Duration");
            dgvHistory.Columns.Add("Barcode", "Barcode");
            dgvHistory.Columns.Add("Result", "Result");
            dgvHistory.Columns.Add("AverageVoltage", "Avg Voltage (V)");
            dgvHistory.Columns.Add("AverageCurrent", "Avg Current (A)");
            dgvHistory.Columns.Add("FilePath", "File Path");

            // Set column widths
            dgvHistory.Columns["Id"].Width = 50;
            dgvHistory.Columns["StartTime"].Width = 150;
            dgvHistory.Columns["EndTime"].Width = 150;
            dgvHistory.Columns["Duration"].Width = 100;
            dgvHistory.Columns["Barcode"].Width = 120;
            dgvHistory.Columns["Result"].Width = 60;
            dgvHistory.Columns["AverageVoltage"].Width = 120;
            dgvHistory.Columns["AverageCurrent"].Width = 120;

            // Style for Result column
            dgvHistory.CellFormatting += DgvHistory_CellFormatting;
        }

        private void DgvHistory_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvHistory.Columns[e.ColumnIndex].Name == "Result" && e.Value != null)
            {
                var result = e.Value.ToString();
                if (result == "OK")
                {
                    e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
                    e.CellStyle.ForeColor = System.Drawing.Color.DarkGreen;
                }
                else if (result == "NG")
                {
                    e.CellStyle.BackColor = System.Drawing.Color.LightCoral;
                    e.CellStyle.ForeColor = System.Drawing.Color.DarkRed;
                }
            }
        }

        private void RefreshHistoryGrid()
        {
            dgvHistory.Rows.Clear();
            var histories = _measurementController.GetAllHistory();

            foreach (var history in histories)
            {
                dgvHistory.Rows.Add(
                    history.Id,
                    history.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    history.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    history.Duration.ToString(@"hh\:mm\:ss"),
                    history.Barcode,
                    history.Result,
                    history.AverageVoltage.ToString("F2"),
                    history.AverageCurrent.ToString("F2"),
                    Path.GetFileName(history.FilePath)
                );
            }
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

            if (!chkDummyMode.Checked)
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

            // Xuất CSV và tạo history record
            await ExportDataAndCreateHistoryAsync();

            // Chuyển sang tab History sau khi xử lý xong
            tabControl1.SelectedIndex = 0; // Index 0 là tabPage1 (History)
        }

        private async Task ExportDataAndCreateHistoryAsync()
        {
            try
            {
                // Generate file path
                var filePath = _reportExporter.GenerateDefaultFilePath();
                
                // Export CSV
                await _measurementController.ExportToCsvAsync(filePath);

                // Create history record (now async)
                var barcode = string.IsNullOrWhiteSpace(TxtBarcode.Text) ? "N/A" : TxtBarcode.Text.Trim();
                var history = await _measurementController.CreateHistoryRecordAsync(barcode, filePath);

                // Refresh history grid
                RefreshHistoryGrid();

                // Clear barcode textbox for next measurement
                TxtBarcode.Clear();

                // Show storage location
                var historyFilePath = _historyRepository.GetStorageFilePath();
                MessageBox.Show($"Data exported and history saved!\nResult: {history.Result}\n\nHistory stored at:\n{historyFilePath}",
                    "Export Success",
                    MessageBoxButtons.OK,
                    history.Result == "OK" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No measurements"))
            {
                // Không có dữ liệu, không lưu history
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

            // Chuyển sang tab Chart
            tabControl1.SelectedIndex = 1; // Index 1 là tabPage2 (Chart)

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
