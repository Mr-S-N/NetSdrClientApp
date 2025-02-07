using BenchmarkDotNet.Running;
using NetSdrClientApp.Interfaces;
using NetSdrClientApp.Services;

using INetSdrClient client = new NetSdrClient();
var cts = new CancellationTokenSource();

try
{
    await client.ConnectAsync(cts.Token);
    await client.SetReceiverFrequencyAsync(100000000, cts.Token);
    await client.StartIQTransmissionAsync(cts.Token);

    Console.WriteLine("Receiving IQ data...");
    await client.ReceiveIQDataAsync("iq_data.bin", cts.Token);

    await client.StopIQTransmissionAsync(cts.Token);
    await client.DisconnectAsync();

    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.ReadKey();
}