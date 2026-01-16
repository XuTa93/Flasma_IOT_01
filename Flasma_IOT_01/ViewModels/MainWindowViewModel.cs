using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flasma_IOT_01.Core.Controllers;
using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Defaults;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flasma_IOT_01.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		public ISeries[] VoltageSeries { get; }
		public ISeries[] CurrentSeries { get; }

        [ObservableProperty]
        private Axis[] voltageXAxes;

        [ObservableProperty]
        private Axis[] voltageYAxes;

        [ObservableProperty]
        private Axis[] currentXAxes;

        [ObservableProperty]
        private Axis[] currentYAxes;

        [ObservableProperty]
		private ObservableCollection<HistoryRecord> historyRecords = new();

		[ObservableProperty]
		private string voltage = "";

		[ObservableProperty]
		private string current = "";

		[ObservableProperty]
		private double alarm;

		[ObservableProperty]
		private double power;

		[ObservableProperty]
		private bool coilDoorClose;

		[ObservableProperty]
		private bool coilDoorOpen;

		[ObservableProperty]
		private bool coilReady;

		[ObservableProperty]
		private bool coilRunning;

		[ObservableProperty]
		private bool coilStop;

		[ObservableProperty]
		private bool isStart = false;

		[ObservableProperty]
		private bool isStop = true;

		[ObservableProperty]
		private bool isConnect = true;

		[ObservableProperty]
		private bool isDisConnect = false;

		[ObservableProperty]
		private bool isDummy;

		[ObservableProperty]
		private string barcodeText = "Code1";

		[ObservableProperty]
		private string statusText = "Ready";

		[ObservableProperty]
		private string statusColor = "Green";

		[ObservableProperty]
		private int okCount;

		[ObservableProperty]
		private int ngCount;

		private bool _startMeasurering = false;

        [ObservableProperty]
		private ObservableCollection<AlarmRecord> alarmRecords = new();


		private readonly MeasurementController _measurementController;
		private readonly ModbusTcpClient _modbusClient;
		private readonly DataSampler _dataSampler;
		private readonly InMemoryDataRepository _dataRepository;
		private readonly ExcelReportExporter _reportExporter;
		private readonly MeasurementHistoryRepository _historyRepository;

		private bool _isRecording = false;
		private int _lastMeasurementCount = 0;
		private DateTime _lastMeasurementTimestamp = DateTime.MinValue;

		// Collections để lưu dữ liệu chart
		private readonly ObservableCollection<double> _voltageValues = new();
		private readonly ObservableCollection<double> _currentValues = new();

		public MainWindowViewModel()
		{
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
            _measurementController.NewSignalReceived += OnNewSignalReceived;
            _measurementController.NewDataRead += OnMeasurementNewDataRead;
			_measurementController.ErrorOccurred += OnMeasurementErrorOccurred;
			_measurementController.MeasurementStarted += OnMeasurementMeasurementStarted;
			_measurementController.MeasurementStopped += OnMeasurementMeasurementStopped;
			_measurementController.ConnectionStatusChanged += OnMeasurementConnectionStatusChanged;

			if (Design.IsDesignMode)
				return;

			// ================= VOLTAGE =================
			VoltageSeries = new ISeries[]
			{
				new LineSeries<double>
				{
					Values = _voltageValues,
					GeometrySize = 0,
					Stroke = new SolidColorPaint(SKColors.Lime, 2),
					Fill = null
				}
			};

			VoltageXAxes = new[]
			{
				new Axis
				{
					MinLimit = 0,
					MaxLimit = 10,
					Name = "Time (s)",
					NameTextSize = 14,
					TextSize = 12,
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
					SeparatorsAtCenter = false,
					Padding = new LiveChartsCore.Drawing.Padding(5, 0, 5, 5),
					ShowSeparatorLines = true
				}
			};

			VoltageYAxes = new[]
			{
				new Axis
				{
					MinLimit = 0,
					MaxLimit = 350,
					Name = "Voltage (V)",
					NameTextSize = 14,
					TextSize = 12,
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
					SeparatorsAtCenter = false,
					Padding = new LiveChartsCore.Drawing.Padding(5, 5, 0, 5),
					ShowSeparatorLines = true
				}
			};

			// ================= CURRENT =================
			CurrentSeries = new ISeries[]
			{
				new LineSeries<double>
				{
					Values = _currentValues,
					GeometrySize = 0,
					Stroke = new SolidColorPaint(SKColors.Cyan, 2),
					Fill = null
				}
			};

			CurrentXAxes = new[]
			{
				new Axis
				{
					MinLimit = 0,
					MaxLimit = 10,
					Name = "Time (s)",
					NameTextSize = 14,
					TextSize = 12,
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
					SeparatorsAtCenter = false,
					Padding = new LiveChartsCore.Drawing.Padding(5, 0, 5, 5),
					ShowSeparatorLines = true
				}
			};

			CurrentYAxes = new[]
			{
				new Axis
				{
					MinLimit = 0,
					MaxLimit = 5,
					Name = "Current (A)",
					NameTextSize = 14,
					TextSize = 12,
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
					SeparatorsAtCenter = false,
					Padding = new LiveChartsCore.Drawing.Padding(5, 5, 0, 5),
					ShowSeparatorLines = true
				}
			};
		}

		private void OnMeasurementConnectionStatusChanged(object? sender, bool e)
		{
			if (!IsDummy)
			{
				if (e == true)
				{
					IsStart = true;
					IsConnect = false;
					IsDisConnect = true;

					// Cập nhật status
					StatusText = "Connected";
					StatusColor = "Green";
				}
				else
				{
					IsStart = false;
					IsConnect = true;

					// Cập nhật status
					StatusText = "Disconnected";
					StatusColor = "Gray";
				}
			}
		}

		[RelayCommand]
		private void OnButtonStartPressed()
		{
			// Thực hiện hành động khi nút được nhấn
			// Xóa dữ liệu cũ trong repository
			_measurementController.ClearMeasurements();

			// Reset cache
			_lastMeasurementCount = 0;
			_lastMeasurementTimestamp = DateTime.MinValue;

			// Bắt đầu recording
			_isRecording = true;

			// Xóa chart
			ClearChart();

			// Cập nhật status
			StatusText = "Recording...";
			StatusColor = "Blue";

			_measurementController.StartMeasurementAsync();
		}

		[RelayCommand]
		private void OnButtonStopPressed()
		{
			// Thực hiện hành động khi nút được nhấn
			_isRecording = false;

			// Cập nhật status
			StatusText = "Stopped";
			StatusColor = "Orange";

			_ = _measurementController.StopMeasurementAsync();
		}

		[RelayCommand]
		private void OnButtonConnectPressed()
		{
			// Thực hiện hành động khi nút được nhấn
			var settings = new ModbusConnectionSettings
			{
				IpAddress = "192.168.1.13",
				Port = 505,
				VoltageRegisterAddress = 3,
				CurrentRegisterAddress = 4,
				PowerRegisterAddress = 5,
				AlarmRegisterAddress = 7,
				CoilDoorCloseRegisterAddress = 0,
				CoilDoorOpenRegisterAddress =1,
                CoilReadyRegisterAddress = 2,
                CoilRunningRegisterAddress = 3,
                CoilStartRegisterAddress = 4,
                CoilStopRegisterAddress = 5,
                SamplingIntervalMs = 1000,
				TimeoutMs = 5000,
				RetryCount = 3
			};

			// Cập nhật status
			StatusText = "Connecting...";
			StatusColor = "Yellow";

			_ = _measurementController.StartBackgroundReadAsync(settings);
		}

		[RelayCommand]
		private void OnButtonDisconnectPressed()
		{
			// Thực hiện hành động khi nút được nhấn
			StatusText = "Disconnecting...";
			StatusColor = "Orange";

			if (_measurementController.IsDummyMode)
			{
				_ = _measurementController.StopDummyModeAsync();
			}
			else
			{
				_ = _measurementController.StopBackgroundReadAsync();
			}
		}

		partial void OnIsDummyChanged(bool value)
		{
			if (value)
			{
				// Enable dummy mode
				IsConnect = false;
				IsStart = true;

				StatusText = "Dummy Mode - Ready";
				StatusColor = "Cyan";

				_measurementController.StartDummyModeAsync(1000);
			}
			else
			{
				// Disable dummy mode
				_measurementController.StopDummyModeAsync();
				IsConnect = !_measurementController.IsModbusConnected;
				IsStart = _measurementController.IsModbusConnected;

				StatusText = "Ready";
				StatusColor = "Green";
			}
		}

		private async void OnMeasurementErrorOccurred(object? sender, string e)
		{
			var box = MessageBoxManager
				.GetMessageBoxStandard("Error", $"Error: {e}", ButtonEnum.Ok, Icon.Error);
			await box.ShowAsync();
		}

		private async void OnMeasurementMeasurementStarted(object? sender, EventArgs e)
		{
			var mode = _measurementController.IsDummyMode ? "Dummy Mode" : "Real Data";
			var box = MessageBoxManager
				.GetMessageBoxStandard("Info", $"Measurement Started ({mode})", ButtonEnum.Ok, Icon.Info);
			await box.ShowAsync();
		}

		private async void OnMeasurementMeasurementStopped(object? sender, EventArgs e)
		{
			var box = MessageBoxManager
				.GetMessageBoxStandard("Info", "Measurement Stopped", ButtonEnum.Ok, Icon.Info);
			await box.ShowAsync();

			// Xuất CSV và tạo history record
			await ExportDataAndCreateHistoryAsync();
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
				var barcode = string.IsNullOrWhiteSpace(BarcodeText) ? "N/A" : BarcodeText.Trim();
				var history = await _measurementController.CreateHistoryRecordAsync(barcode, filePath);

				// Refresh history grid
				RefreshHistoryGrid();

				// Clear barcode textbox for next measurement
				BarcodeText = "";

				// Show storage location
				var historyFilePath = _historyRepository.GetStorageFilePath();
				var box = MessageBoxManager
					.GetMessageBoxStandard("Export Success",
						$"Data exported and history saved!\nResult: {history.Result}\n\nHistory stored at:\n{historyFilePath}",
						ButtonEnum.Ok,
						history.Result == "OK" ? Icon.Info : Icon.Warning);
				await box.ShowAsync();
			}
			catch (InvalidOperationException ex) when (ex.Message.Contains("No measurements"))
			{
				// Không có dữ liệu, không lưu history
			}
			catch (Exception ex)
			{
				var box = MessageBoxManager
					.GetMessageBoxStandard("Export failed:", $"Export Error: {ex}", ButtonEnum.Ok, Icon.Error);
				await box.ShowAsync();
			}
		}

		private void RefreshHistoryGrid()
		{
			HistoryRecords.Clear();

			int ok = 0;
			int ng = 0;

			var histories = _measurementController.GetAllHistory();
			foreach (var h in histories)
			{
				HistoryRecords.Add(new HistoryRecord
				{
					Id = h.Id,
					StartTime = h.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
					EndTime = h.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
					Duration = h.Duration.ToString(@"hh\:mm\:ss"),
					Barcode = h.Barcode,
					Result = h.Result,
					AvgVoltage = h.AverageVoltage,
					AvgCurrent = h.AverageCurrent,
					FilePath = Path.GetFileName(h.FilePath)
				});

				// Đếm OK / NG
				if (h.Result.Equals("OK", StringComparison.OrdinalIgnoreCase))
					ok++;
				else
					ng++;
			}

			OkCount = ok;
			NgCount = ng;
		}
        
        private void OnNewSignalReceived(object? sender, NewSignalEventArgs e)
		{

            // Cập nhật coil status với tên đúng
            CoilDoorClose = e.IsDoorClosed;    // <-- IsDoorClosed
            CoilDoorOpen = e.IsDoorOpened;     // <-- IsDoorOpened
            CoilReady = e.IsReady;             // <-- IsReady
            CoilRunning = e.IsRunning;         // <-- IsRunning
            CoilStop = e.IsStopped;            // <-- IsStopped
            _startMeasurering = e.IsStart;     // <-- IsStart

			if (_startMeasurering && _isRecording == false)
			{
				ClearChart();
                OnButtonStartPressed();
            }
			if (CoilStop && _isRecording)
			{
				OnButtonStopPressed();
            }

            UpdateAlarmStatus((int)e.AlarmStatus);

            // Sửa thành AlarmStatus (thay vì Alarm)
            Alarm = e.AlarmStatus; // <-- SỬA TỪ e.Alarm THÀNH e.AlarmStatus

            // Sửa thành PowerSetting (thay vì Power)
            Power = e.PowerSetting; // <-- SỬA TỪ e.Power THÀNH e.PowerSetting
        }
        private void OnMeasurementNewDataRead(object? sender, NewDataReadEventArgs e)
		{
			// ================= SCALE DATA =================
			double voltageScaled = e.Voltage / 10.0;     // VD: 3250 -> 325.0V
			double currentScaled = e.Current / 1000.0;   // VD: 2350 -> 2.350A

			// ================= UI TEXT ====================
			Voltage = voltageScaled.ToString("F1");  // 1 số sau dấu chấm
			Current = currentScaled.ToString("F3");  // 3 số sau dấu chấm


			// Chỉ vẽ chart khi đang recording (từ Start đến Stop)
			if (_isRecording)
			{
				// Thêm dữ liệu mới vào chart
				_voltageValues.Add(e.Voltage);
				_currentValues.Add(e.Current);

				// Cập nhật trục X để hiển thị toàn bộ dữ liệu
				VoltageXAxes[0].MinLimit = 0;
				VoltageXAxes[0].MaxLimit = _voltageValues.Count > 10 ? _voltageValues.Count : 10;

				CurrentXAxes[0].MinLimit = 0;
				CurrentXAxes[0].MaxLimit = _currentValues.Count > 10 ? _currentValues.Count : 10;

				// Tự động điều chỉnh trục Y cho Voltage
				if (_voltageValues.Any())
				{
					var maxVoltage = _voltageValues.Max();
					var minVoltage = _voltageValues.Min();

					var range = maxVoltage - minVoltage;
					const double minVoltageSpan = 0.5;
					var padding = Math.Max(range * 0.1, minVoltageSpan);

					var suggestedMin = Math.Floor(minVoltage - padding / 2.0);
					suggestedMin = Math.Max(0, suggestedMin);
					var suggestedMax = Math.Ceiling(maxVoltage + padding / 2.0);

					if (suggestedMax <= suggestedMin)
					{
						suggestedMax = suggestedMin + minVoltageSpan;
					}

					VoltageYAxes[0].MinLimit = suggestedMin;
					VoltageYAxes[0].MaxLimit = suggestedMax;
				}
				else
				{
					VoltageYAxes[0].MinLimit = 0;
					VoltageYAxes[0].MaxLimit = 350;
				}

				// Tự động điều chỉnh trục Y cho Current
				if (_currentValues.Any())
				{
					var maxCurrent = _currentValues.Max();
					var minCurrent = _currentValues.Min();

					var range = maxCurrent - minCurrent;
					const double minCurrentSpan = 0.5;
					var padding = Math.Max(range * 0.1, minCurrentSpan);

					var suggestedMin = Math.Floor(minCurrent - padding / 2.0);
					suggestedMin = Math.Max(0, suggestedMin);
					var suggestedMax = Math.Ceiling(maxCurrent + padding / 2.0);

					if (suggestedMax <= suggestedMin)
					{
						suggestedMax = suggestedMin + minCurrentSpan;
					}

					CurrentYAxes[0].MinLimit = suggestedMin;
					CurrentYAxes[0].MaxLimit = suggestedMax;
				}
				else
				{
					CurrentYAxes[0].MinLimit = 0;
					CurrentYAxes[0].MaxLimit = 5;
				}
			}
		}

		private void ClearChart()
		{
			_voltageValues.Clear();
			_currentValues.Clear();

			// Reset trục X
			VoltageXAxes[0].MinLimit = 0;
			VoltageXAxes[0].MaxLimit = 10;
			CurrentXAxes[0].MinLimit = 0;
			CurrentXAxes[0].MaxLimit = 10;

			// Reset trục Y về giá trị mặc định
			VoltageYAxes[0].MinLimit = 0;
			VoltageYAxes[0].MaxLimit = 350;
			CurrentYAxes[0].MinLimit = 0;
			CurrentYAxes[0].MaxLimit = 5;
		}
		private int _alarmStatus;

		private void UpdateAlarmStatus(int alarmStatus)
		{
			if (_alarmStatus == alarmStatus)
				return;

			_alarmStatus = alarmStatus;
			AlarmRecords.Clear();

			if (_alarmStatus == 0)
			{
				AlarmRecords.Add(new AlarmRecord
				{
					No = 1,
					NameAlarm = "No Alarm",
					Status = "Đã hết lỗi",
					StatusColor = "Lime",
					StatusFontWeight = "Bold",
					Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				return;
			}

			if ((_alarmStatus & 1) == 1)
				AddAlarm("Overcurrent");

			if ((_alarmStatus & 2) == 2)
				AddAlarm("Overtemperature");

			if ((_alarmStatus & 4) == 4)
				AddAlarm("Overvoltage");
			if ((_alarmStatus & 8) == 8)
				AddAlarm("Undervoltage");
			if ((_alarmStatus & 16) == 16)
				AddAlarm ("Air Pressure");
			if ((_alarmStatus & 32) == 32)
				AddAlarm(("Open Circuit"));
			if ((_alarmStatus & 64) == 64)
				AddAlarm("IGBT1 Overcurrent ");
			if ((_alarmStatus & 128) == 128)
				AddAlarm("IGBT2 Overcurrent");
			if ((_alarmStatus & 256) == 256)
				AddAlarm("Inverter overcurrent");
			if ((_alarmStatus & 512) == 512)
				AddAlarm("No arcing alarm");
			if ((_alarmStatus & 1024) == 1024)
				AddAlarm("Motor non - rotation alarm");
		}

		private void AddAlarm(string name)
		{
			AlarmRecords.Add(new AlarmRecord
			{
				No = AlarmRecords.Count + 1,
				NameAlarm = name,
				Status = "Bị lỗi",
				StatusColor = "Red",
				StatusFontWeight = "Bold",
				Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			});
		}


	}

	public class AlarmRecord
	{
		public int No { get; set; }
		public string NameAlarm { get; set; } = "";
		public string Status { get; set; } = "";
		public string StatusColor { get; set; } = "Green";
		public string StatusFontWeight { get; set; } = "Bold";
		public string Timestamp { get; set; } = "";
	}
	// Model class cho History Record
	public class HistoryRecord
	{
		public int Id { get; set; }
		public string StartTime { get; set; } = string.Empty;
		public string EndTime { get; set; } = string.Empty;
		public string Duration { get; set; } = string.Empty;
		public string Barcode { get; set; } = string.Empty;
		public string Result { get; set; } = string.Empty;
		public double AvgVoltage { get; set; }
		public double AvgCurrent { get; set; }
		public string FilePath { get; set; } = string.Empty;
	}


}