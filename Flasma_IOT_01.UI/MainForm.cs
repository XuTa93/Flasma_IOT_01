using Flasma_IOT_01.Core.Controllers;
using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;

namespace Flasma_IOT_01.UI
{
    public partial class MainForm : Form
    {
        private readonly MeasurementController _measurementController;
        private readonly ModbusTcpClient _modbusClient;
        private readonly DataSampler _dataSampler;
        private readonly InMemoryDataRepository _dataRepository;
        private readonly ExcelReportExporter _reportExporter;
        public MainForm()
        {
            InitializeComponent();
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
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            using var client = new ModbusTcpClient();
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
            _measurementController.StopBackgroundReadAsync();
        }

        private void OnMeasurementControllerNewDataRead(object? sender, NewDataReadEventArgs e)
        {
            // Handle new data read from Modbus
            // This is called every time voltage and current are read
            this.Invoke(() =>
            {
                // Update UI with voltage and current values
                lblCurentVolt.Text = $"Voltage: {e.Voltage:F2}V";
                lblCurrentAmpe.Text = $"Current: {e.Current:F2}A";
            });
        }

        private void OnMeasurementControllerErrorOccurred(object? sender, string e)
        {
            this.Invoke(() =>
            {
                MessageBox.Show($"Error: {e}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private void OnMeasurementControllerMeasurementStarted(object? sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                MessageBox.Show("Measurement Started", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void OnMeasurementControllerMeasurementStopped(object? sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                MessageBox.Show("Measurement Stopped", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _measurementController.StartMeasurementAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _measurementController.StopMeasurementAsync();
        }

        /// <summary>
        /// Lấy dữ liệu đo lường từ kho dữ liệu và hiển thị trên giao diện người dùng
        /// </summary>

        private void btnTestData_Click(object sender, EventArgs e)
        {
            var data = _measurementController.GetAllMeasurements();
        }
    }
}
