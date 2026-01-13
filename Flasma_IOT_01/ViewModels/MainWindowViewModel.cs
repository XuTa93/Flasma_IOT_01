using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flasma_IOT_01.Core.Models;

namespace Flasma_IOT_01.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
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
