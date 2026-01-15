namespace Flasma_IOT_01.Core.Models
{
    /// <summary>
    /// Event args for when new data is read from Modbus
    /// </summary>
    public class NewDataReadEventArgs : EventArgs
    {
        public double Voltage { get; set; }
        public double Current { get; set; }
        public DateTime Timestamp { get; set; }

        public NewDataReadEventArgs(double voltage, double current)
        {
            Voltage = voltage;
            Current = current;
            Timestamp = DateTime.UtcNow;
        }
    }
    public class NewSignalEventArgs : EventArgs
    {
        public double PowerSetting { get; set; }
        public double AlarmStatus { get; set; }
        public bool IsDoorClosed { get; set; } = false;
        public bool IsDoorOpened { get; set; } = false;
        public bool IsReady { get; set; } = false;
        public bool IsRunning { get; set; } = false;

        public bool IsStart { get; set; } = false;
        public bool IsStopped { get; set; } = false;

        public NewSignalEventArgs(double powerSetting, double alarmStatus, bool isDoorClosed, bool isDoorOpened, bool isReady, bool isRunning, bool isStart, bool isStopped)
        {
            PowerSetting = powerSetting;
            AlarmStatus = alarmStatus;
            IsDoorClosed = isDoorClosed;
            IsDoorOpened = isDoorOpened;
            IsReady = isReady;
            IsRunning = isRunning;
            IsStopped = isStopped;
            IsStart = isStart;
        }
    }
}
