namespace Flasma_IOT_01.Core.Models
{
    /// <summary>
    /// Event args for when new data is read from Modbus
    /// </summary>
    public class NewDataReadEventArgs : EventArgs
    {
        public double Voltage { get; set; }
        public double Current { get; set; }

        public double PowerSetting { get; set; }
        public double AlarmStatus { get; set; }
        public bool IsDoorClosed { get; set; }= false;
        public bool IsDoorOpened { get; set; } = false;
        public bool IsReady { get; set; } = false;
        public bool IsRunning { get; set; } = false;
        public bool IsStopped { get; set; } = false;

        public DateTime Timestamp { get; set; }

        public NewDataReadEventArgs(double voltage, double current, double powerSetting, double alarmStatus, bool isDoorClosed, bool isDoorOpened, bool isReady, bool isRunning, bool isStopped)
        {
            Voltage = voltage;
            Current = current;
            PowerSetting = powerSetting;
            AlarmStatus = alarmStatus;
            IsDoorClosed = isDoorClosed;
            IsDoorOpened = isDoorOpened;
            IsReady = isReady;
            IsRunning = isRunning;
            IsStopped = isStopped;
            Timestamp = DateTime.UtcNow;
        }
    }
}
