using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flasma_IOT_01.Core.Models;
using LiveChartsCore.SkiaSharpView;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

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

		public MainWindowViewModel()
		{
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
            // Chỉ để LiveCharts tự động tạo labels
            // Labels = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
			SeparatorsAtCenter = false,
			Padding = new LiveChartsCore.Drawing.Padding(5, 0, 5, 5),
			ShowSeparatorLines = true
            // Xóa ForceStepToMin và MinStep
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
            // Chỉ để LiveCharts tự động tạo labels với khoảng cách hợp lý
            // Labels = new[] { "0", "50", "100", "150", "200", "250", "300", "350" },
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
			SeparatorsAtCenter = false,
			Padding = new LiveChartsCore.Drawing.Padding(5, 5, 0, 5),
			ShowSeparatorLines = true
            // Xóa ForceStepToMin và MinStep
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
            // Chỉ để LiveCharts tự động tạo labels
            // Labels = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
			SeparatorsAtCenter = false,
			Padding = new LiveChartsCore.Drawing.Padding(5, 0, 5, 5),
			ShowSeparatorLines = true
            // Xóa ForceStepToMin và MinStep
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
            // Chỉ để LiveCharts tự động tạo labels
            // Labels = new[] { "0", "1", "2", "3", "4", "5" },
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
			SeparatorsAtCenter = false,
			Padding = new LiveChartsCore.Drawing.Padding(5, 5, 0, 5),
			ShowSeparatorLines = true
            // Xóa ForceStepToMin và MinStep
        }
	};
		}



		[ObservableProperty]
        private double voltage;

        [ObservableProperty]
        private double current;

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

        public string Greeting { get; } = "Welcome to Avalonia!";

        [RelayCommand]
        private void OnButtonStartPressed()
        {
            // Thực hiện hành động khi nút được nhấn
            System.Diagnostics.Debug.WriteLine("Button pressed!");
        }


	}
}
