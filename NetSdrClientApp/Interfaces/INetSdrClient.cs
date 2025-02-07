namespace NetSdrClientApp.Interfaces
{
    public interface INetSdrClient : IDisposable
    {
        /// <summary>
        /// Checks if the client is connected to the NetSDR device.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Establishes a connection to the NetSDR device.
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects from the NetSDR device.
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Starts the IQ data transmission from the NetSDR device.
        /// </summary>
        Task StartIQTransmissionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the IQ data transmission from the NetSDR device.
        /// </summary>
        Task StopIQTransmissionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the receiver frequency on the NetSDR device.
        /// </summary>
        /// <param name="frequency">The desired frequency in Hz.</param>
        Task SetReceiverFrequencyAsync(int frequency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Receives IQ data from the NetSDR device and writes it to a file.
        /// </summary>
        /// <param name="filePath">The path where the IQ data should be saved.</param>
        Task ReceiveIQDataAsync(string filePath, CancellationToken cancellationToken = default);
    }
}

