using Flasma_IOT_01.Core.Models;
using Modbus.Device;
using System.Net.Sockets;

namespace Flasma_IOT_01.Core.Services;

/// <summary>
/// Modbus TCP client implementation
/// </summary>
public class ModbusTcpClient : IDisposable
{
    private TcpClient? _tcpClient;
    private IModbusMaster? _modbusMaster;
    private bool _isConnected;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _currentIpAddress;
    private int _currentPort;

    public bool IsConnected => _isConnected && _tcpClient?.Connected == true;

    public async Task ConnectAsync(string ipAddress, int port)
    {
        await _lock.WaitAsync();
        try
        {
            if (IsConnected && _currentIpAddress == ipAddress && _currentPort == port)
            {
                return;
            }

            await DisconnectInternalAsync();

            _tcpClient = new TcpClient
            {
                ReceiveTimeout = 5000,
                SendTimeout = 5000
            };

            await _tcpClient.ConnectAsync(ipAddress, port);
            _modbusMaster = ModbusIpMaster.CreateIp(_tcpClient);
            
            _currentIpAddress = ipAddress;
            _currentPort = port;
            _isConnected = true;
        }
        catch
        {
            await DisconnectInternalAsync();
            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DisconnectAsync()
    {
        await _lock.WaitAsync();
        try
        {
            await DisconnectInternalAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task DisconnectInternalAsync()
    {
        _isConnected = false;
        
        if (_modbusMaster != null)
        {
            _modbusMaster.Dispose();
            _modbusMaster = null;
        }

        if (_tcpClient != null)
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
            _tcpClient = null;
        }

        _currentIpAddress = null;
        _currentPort = 0;
        
        await Task.CompletedTask;
    }

    public async Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort quantity)
    {
        if (!IsConnected || _modbusMaster == null)
        {
            throw new InvalidOperationException("Modbus client is not connected");
        }

        await _lock.WaitAsync();
        try
        {
            return await _modbusMaster.ReadHoldingRegistersAsync(1, startAddress, quantity);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task WriteRegisterAsync(ushort address, ushort value)
    {
        if (!IsConnected || _modbusMaster == null)
        {
            throw new InvalidOperationException("Modbus client is not connected");
        }

        await _lock.WaitAsync();
        try
        {
            await _modbusMaster.WriteSingleRegisterAsync(1, address, value);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort quantity)
    {
        if (!IsConnected || _modbusMaster == null)
        {
            throw new InvalidOperationException("Modbus client is not connected");
        }

        await _lock.WaitAsync();
        try
        {
            return await _modbusMaster.ReadInputRegistersAsync(1, startAddress, quantity);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] values)
    {
        if (!IsConnected || _modbusMaster == null)
        {
            throw new InvalidOperationException("Modbus client is not connected");
        }

        await _lock.WaitAsync();
        try
        {
            await _modbusMaster.WriteMultipleRegistersAsync(1, startAddress, values);
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _lock.Wait();
        try
        {
            DisconnectInternalAsync().GetAwaiter().GetResult();
            _lock.Dispose();
        }
        catch
        {
            // Suppress exceptions during disposal
        }
    }
}
