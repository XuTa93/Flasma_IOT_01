using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flasma_IOT_01.Core.Controllers;
using Flasma_IOT_01.Core.Models;
using Flasma_IOT_01.Core.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flasma_IOT_01.Views
{
	public partial class MainWindow : Window
	{
		// ===== CORE CONTROLLERS =====
		private readonly MeasurementController _measurementController;
		private readonly ModbusTcpClient _modbusClient;
		private readonly DataSampler _dataSampler;
		private readonly InMemoryDataRepository _dataRepository;
		private readonly ExcelReportExporter _reportExporter;
		private readonly MeasurementHistoryRepository _historyRepository;

		// ===== STATE =====
		private bool _isRecording = false;
		private int _lastMeasurementCount = 0;
		private DateTime _lastMeasurementTimestamp = DateTime.MinValue;

		// ===== CHART CONTROLS =====
		private CartesianChart? _chartVolt;
		private CartesianChart? _chartCurrent;

		// ===== CHART DATA =====
		private readonly List<double> _time = new();
		private readonly List<double> _voltages = new();
		private readonly List<double> _currents = new();

		// =====================================================
		// CONSTRUCTOR
		// =====================================================
		public MainWindow()
		{
			InitializeComponent();

			// ---- init core services (GIỮ NGUYÊN LOGIC CŨ) ----
			_modbusClient = new ModbusTcpClient();
			_dataSampler = new DataSampler();
			_dataRepository = new InMemoryDataRepository();
			_reportExporter = new ExcelReportExporter();
			_historyRepository = new MeasurementHistoryRepository();

			_measurementController = new MeasurementController(
				_modbusClient,
				_dataSampler,
				_dataRepository,
				_reportExporter,
				_historyRepository);

			// ---- subscribe events ----
			_measurementController.NewDataRead += OnMeasurementControllerNewDataRead;
			_measurementController.MeasurementStarted += OnMeasurementControllerMeasurementStarted;
			_measurementController.MeasurementStopped += OnMeasurementControllerMeasurementStopped;
			_measurementController.ErrorOccurred += OnMeasurementControllerErrorOccurred;

			// ---- init charts ----
			InitializeCharts();
		}

		// =====================================================
		// XAML LOADER
		// =====================================================
		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		// =====================================================
		// INIT CHARTS (TƯƠNG ĐƯƠNG InitializeCharts() WINFORMS)
		// =====================================================
		private void InitializeCharts()
		{
			_chartVolt = this.FindControl<CartesianChart>("Chart_Volt");
			_chartCurrent = this.FindControl<CartesianChart>("Chart_Current");

			InitVoltageChart();
			InitCurrentChart();
		}

		private void InitVoltageChart()
		{
			if (_chartVolt == null) return;

			_chartVolt.Series = new ISeries[]
			{
				new LineSeries<double>
				{
					Values = _voltages,
					Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0
				}
			};

			_chartVolt.XAxes = new[]
			{
				new Axis
				{
					Name = "Time (s)",
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.DimGray)
				}
			};

			_chartVolt.YAxes = new[]
			{
				new Axis
				{
					Name = "Voltage (V)",
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.DimGray)
				}
			};
		}

		private void InitCurrentChart()
		{
			if (_chartCurrent == null) return;

			_chartCurrent.Series = new ISeries[]
			{
				new LineSeries<double>
				{
					Values = _currents,
					Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0
				}
			};

			_chartCurrent.XAxes = new[]
			{
				new Axis
				{
					Name = "Time (s)",
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.DimGray)
				}
			};

			_chartCurrent.YAxes = new[]
			{
				new Axis
				{
					Name = "Current (A)",
					LabelsPaint = new SolidColorPaint(SKColors.White),
					SeparatorsPaint = new SolidColorPaint(SKColors.DimGray)
				}
			};
		}

		// =====================================================
		// DATA UPDATE (TƯƠNG ĐƯƠNG UpdateChartsFromRepository)
		// =====================================================
		private void UpdateChartsFromRepository()
		{
			var measurements = _measurementController.GetAllMeasurements().ToList();
			if (!measurements.Any()) return;

			var currentCount = measurements.Count;
			var lastTimestamp = measurements.Last().Timestamp;

			if (currentCount == _lastMeasurementCount &&
				lastTimestamp == _lastMeasurementTimestamp)
				return;

			_lastMeasurementCount = currentCount;
			_lastMeasurementTimestamp = lastTimestamp;

			_time.Clear();
			_voltages.Clear();
			_currents.Clear();

			var startTime = measurements.First().Timestamp;

			foreach (var m in measurements)
			{
				_time.Add((m.Timestamp - startTime).TotalSeconds);
				_voltages.Add(m.Voltage);
				_currents.Add(m.Current);
			}

			// LiveCharts auto refresh khi Values thay đổi
			_chartVolt?.InvalidateVisual();
			_chartCurrent?.InvalidateVisual();
		}

		// =====================================================
		// EVENTS (GIỮ LOGIC WINFORMS)
		// =====================================================
		private void OnMeasurementControllerNewDataRead(object? sender, NewDataReadEventArgs e)
		{
			if (_isRecording)
			{
				UpdateChartsFromRepository();
			}
		}

		private void OnMeasurementControllerMeasurementStarted(object? sender, EventArgs e)
		{
			_isRecording = true;

			_lastMeasurementCount = 0;
			_lastMeasurementTimestamp = DateTime.MinValue;

			_time.Clear();
			_voltages.Clear();
			_currents.Clear();

			_chartVolt?.InvalidateVisual();
			_chartCurrent?.InvalidateVisual();
		}

		private async void OnMeasurementControllerMeasurementStopped(object? sender, EventArgs e)
		{
			_isRecording = false;

			await _measurementController.ExportToCsvAsync(
				_reportExporter.GenerateDefaultFilePath());
		}

		private void OnMeasurementControllerErrorOccurred(object? sender, string e)
		{
			Console.WriteLine($"Error: {e}");
		}
	}
}
