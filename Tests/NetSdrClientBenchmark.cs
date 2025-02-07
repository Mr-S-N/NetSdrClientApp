using System;
using BenchmarkDotNet.Attributes;
using NetSdrClientApp.Interfaces;
using NetSdrClientApp.Services;

namespace Tests
{
    [MemoryDiagnoser]
    public class NetSdrClientBenchmark
    {
        private readonly INetSdrClient _client;

        public NetSdrClientBenchmark()
        {
            _client = new NetSdrClient("127.0.0.1", 50000);
        }

        [GlobalSetup]
        public async Task Setup()
        {
            await _client.ConnectAsync();
        }

        [GlobalCleanup]
        public async Task Cleanup()
        {
            await _client.DisconnectAsync();
            _client.Dispose();
        }

        [Benchmark]
        public async Task Connect_Disconnect()
        {
            await _client.ConnectAsync();
            await _client.DisconnectAsync();
        }

        [Benchmark]
        public async Task SetReceiverFrequency()
        {
            await _client.SetReceiverFrequencyAsync(100000000);
        }

        [Benchmark]
        public async Task Start_Stop_IQ_Transmission()
        {
            await _client.StartIQTransmissionAsync();
            await _client.StopIQTransmissionAsync();
        }

        [Benchmark]
        public async Task ReceiveIQData()
        {
            string filePath = Path.GetTempFileName();
            await _client.ReceiveIQDataAsync(filePath);
            File.Delete(filePath);
        }
    }
}

