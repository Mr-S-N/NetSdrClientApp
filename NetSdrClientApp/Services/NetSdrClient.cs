using NetSdrClientApp.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace NetSdrClientApp.Services
{
    public class NetSdrClient : INetSdrClient
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private bool _disposed;

        public bool IsConnected => _client?.Connected == true;

        public NetSdrClient(string host = "127.0.0.1", int port = 50000)
        {
            _host = host;
            _port = port;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                Console.WriteLine("[INFO] Already connected to NetSDR.");
                return;
            }

            try
            {
                _client = new TcpClient();
                Console.WriteLine($"[INFO] Connecting to NetSDR at {_host}:{_port}...");
                await _client.ConnectAsync(_host, _port, cancellationToken);
                _stream = _client.GetStream();
                Console.WriteLine("[INFO] Successfully connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to connect: {ex.Message}");
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_client is not { Connected: true }) return;

            Console.WriteLine("[INFO] Disconnecting from NetSDR...");
            _client.Close();
            _client.Dispose();
            _client = null;
            _stream = null;
            Console.WriteLine("[INFO] Disconnected successfully.");
        }

        private async Task SendCommandAsync(string command, CancellationToken cancellationToken = default)
        {
            if (_stream is null || _client is null || !_client.Connected)
            {
                Console.WriteLine("[WARNING] Attempted to send command while not connected.");
                throw new InvalidOperationException("Not connected to NetSDR.");
            }

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(command + "\n");
                await _stream.WriteAsync(data, 0, data.Length, cancellationToken);
                Console.WriteLine($"[INFO] Sent command: {command}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send command: {ex.Message}");
                throw;
            }
        }

        public async Task StartIQTransmissionAsync(CancellationToken cancellationToken = default)
            => await SendCommandAsync("START_IQ", cancellationToken);

        public async Task StopIQTransmissionAsync(CancellationToken cancellationToken = default)
            => await SendCommandAsync("STOP_IQ", cancellationToken);

        public async Task SetReceiverFrequencyAsync(int frequency, CancellationToken cancellationToken = default)
            => await SendCommandAsync($"SET_FREQ {frequency}", cancellationToken);

        public async Task ReceiveIQDataAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (_stream is null || _client is null || !_client.Connected)
            {
                Console.WriteLine("[WARNING] Attempted to receive data while not connected.");
                throw new InvalidOperationException("Not connected to NetSDR.");
            }

            try
            {
                await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                byte[] buffer = new byte[8192];

                Console.WriteLine("[INFO] Receiving IQ data...");
                while (!cancellationToken.IsCancellationRequested && _client.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                }

                Console.WriteLine($"[INFO] IQ data successfully received and saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error receiving IQ data: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            Console.WriteLine("[INFO] Disposing NetSdrClient...");
            _client?.Close();
            _client?.Dispose();
            _disposed = true;
            Console.WriteLine("[INFO] Disposed successfully.");
        }
    }
}