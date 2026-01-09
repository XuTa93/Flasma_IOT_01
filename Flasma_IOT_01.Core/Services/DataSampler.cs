using Flasma_IOT_01.Core.Models;

namespace Flasma_IOT_01.Core.Services;

public delegate void DataSamplingEventHandler(object sender, EventArgs e);

/// <summary>
/// Data sampler implementation with timer-based scheduling (1 second intervals)
/// </summary>
public class DataSampler
{
    private Timer? _timer;
    private bool _isRunning;

    public event DataSamplingEventHandler? DataSampled;

    public bool IsRunning => _isRunning;

    public void Start(int samplingIntervalMs)
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _timer = new Timer(
            callback: _ => OnDataSampled(),
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromMilliseconds(samplingIntervalMs)
        );
    }

    public void Stop()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _timer?.Dispose();
        _timer = null;
    }

    private void OnDataSampled()
    {
        DataSampled?.Invoke(this, EventArgs.Empty);
    }
}
