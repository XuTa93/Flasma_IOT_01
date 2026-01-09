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
}
