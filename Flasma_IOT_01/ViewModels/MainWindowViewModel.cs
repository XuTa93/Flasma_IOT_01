using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flasma_IOT_01.Core.Models;
using LiveChartsCore.SkiaSharpView;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Flasma_IOT_01.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		public ISeries[] VoltageSeries { get; }
		public ISeries[] CurrentSeries { get; }

		public Axis[] VoltageXAxes { get; }
		public Axis[] VoltageYAxes { get; }

		public Axis[] CurrentXAxes { get; }
		public Axis[] CurrentYAxes { get; }

		[ObservableProperty]
		private ObservableCollection<HistoryRecord> historyRecords = new();

		[ObservableProperty]
		private double voltage = 220.63;

		[ObservableProperty]
		private double current = 11.60;

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

		public MainWindowViewModel()
		{
			// Khởi tạo dữ liệu mẫu ngay cả trong design mode
			InitializeSampleData();

			if (Design.IsDesignMode)
				return;

			// ================= VOLTAGE =================
			VoltageSeries = new ISeries[]
			{
				new LineSeries<double>
				{
					Values = new double[] { 210, 220, 215, 230, 225 },
					GeometrySize = 0,
					Stroke = new SolidColorPaint(SKColors.Lime, 2)
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
					Values = new double[] { 1.2, 1.5, 1.8, 2.0, 1.6 },
					GeometrySize = 0,
					Stroke = new SolidColorPaint(SKColors.Cyan, 2)
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

		private void InitializeSampleData()
		{
			// Thêm dữ liệu mẫu cho DataGrid
			HistoryRecords.Add(new HistoryRecord
			{
				Id = 1,
				StartTime = "2024-01-15 08:30:00",
				EndTime = "2024-01-15 08:45:00",
				Duration = "00:15:00",
				Barcode = "ABC123456",
				Result = "PASS",
				AvgVoltage = 220.63,
				AvgCurrent = 11.60,
				FilePath = "C:\\Data\\test1.csv"
			});

			HistoryRecords.Add(new HistoryRecord
			{
				Id = 2,
				StartTime = "2024-01-15 09:00:00",
				EndTime = "2024-01-15 09:20:00",
				Duration = "00:20:00",
				Barcode = "DEF789012",
				Result = "FAIL",
				AvgVoltage = 218.50,
				AvgCurrent = 10.80,
				FilePath = "C:\\Data\\test2.csv"
			});

			HistoryRecords.Add(new HistoryRecord
			{
				Id = 3,
				StartTime = "2024-01-15 10:15:00",
				EndTime = "2024-01-15 10:30:00",
				Duration = "00:15:00",
				Barcode = "GHI345678",
				Result = "PASS",
				AvgVoltage = 221.20,
				AvgCurrent = 12.10,
				FilePath = "C:\\Data\\test3.csv"
			});

			HistoryRecords.Add(new HistoryRecord
			{
				Id = 4,
				StartTime = "2024-01-15 11:00:00",
				EndTime = "2024-01-15 11:25:00",
				Duration = "00:25:00",
				Barcode = "JKL901234",
				Result = "PASS",
				AvgVoltage = 219.80,
				AvgCurrent = 11.30,
				FilePath = "C:\\Data\\test4.csv"
			});
		}

		[RelayCommand]
		private void OnButtonStartPressed()
		{
			// Thực hiện hành động khi nút được nhấn
			System.Diagnostics.Debug.WriteLine("Button pressed!");
		}
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