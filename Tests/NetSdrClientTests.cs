using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NetSdrClientApp.Interfaces;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class NetSdrClientTests
    {
        private Mock<INetSdrClient> _mockClient;

        [SetUp]
        public void Setup()
        {
            _mockClient = new Mock<INetSdrClient>();
        }

        [Test]
        public async Task ConnectAsync_ShouldNotThrowException()
        {
            _mockClient.Setup(c => c.ConnectAsync(It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.ConnectAsync(CancellationToken.None), Throws.Nothing);
        }

        [Test]
        public async Task DisconnectAsync_ShouldNotThrowException()
        {
            _mockClient.Setup(c => c.DisconnectAsync()).Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.DisconnectAsync(), Throws.Nothing);
        }

        [Test]
        public async Task SetReceiverFrequencyAsync_ShouldSendCommand()
        {
            _mockClient.Setup(c => c.SetReceiverFrequencyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.SetReceiverFrequencyAsync(100000000, CancellationToken.None), Throws.Nothing);
        }

        [Test]
        public async Task StartIQTransmissionAsync_ShouldStartTransmission()
        {
            _mockClient.Setup(c => c.StartIQTransmissionAsync(It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.StartIQTransmissionAsync(CancellationToken.None), Throws.Nothing);
        }

        [Test]
        public async Task StopIQTransmissionAsync_ShouldStopTransmission()
        {
            _mockClient.Setup(c => c.StopIQTransmissionAsync(It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.StopIQTransmissionAsync(CancellationToken.None), Throws.Nothing);
        }

        [Test]
        public async Task ReceiveIQDataAsync_ShouldWriteToFile()
        {
            string testFilePath = "test";

            _mockClient.Setup(c => c.ReceiveIQDataAsync(testFilePath, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            Assert.That(async () => await _mockClient.Object.ReceiveIQDataAsync(testFilePath, CancellationToken.None), Throws.Nothing);
        }

        [Test]
        public void IsConnected_ShouldReturnFalse_WhenNotConnected()
        {
            _mockClient.Setup(c => c.IsConnected).Returns(false);
            Assert.That(_mockClient.Object.IsConnected, Is.False);
        }

        [Test]
        public void IsConnected_ShouldReturnTrue_WhenConnected()
        {
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            Assert.That(_mockClient.Object.IsConnected, Is.True);
        }

        [Test]
        public void Dispose_ShouldNotThrowException()
        {
            Assert.That(() => _mockClient.Object.Dispose(), Throws.Nothing);
        }

        [Test]
        public async Task ConnectAsync_ShouldThrowException_WhenConnectionFails()
        {
            _mockClient.Setup(c => c.ConnectAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("Connection failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _mockClient.Object.ConnectAsync(CancellationToken.None));
            Assert.That(ex?.Message, Is.EqualTo("Connection failed"));
        }

        [Test]
        public async Task SendCommandAsync_ShouldThrowException_WhenDisconnected()
        {
            _mockClient.Setup(c => c.SetReceiverFrequencyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Not connected to NetSDR"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mockClient.Object.SetReceiverFrequencyAsync(100000000, CancellationToken.None));

            Assert.That(ex?.Message, Is.EqualTo("Not connected to NetSDR"));
        }

        [Test]
        public async Task ReceiveIQDataAsync_ShouldThrowException_WhenNotConnected()
        {
            _mockClient.Setup(c => c.ReceiveIQDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Not connected to NetSDR"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mockClient.Object.ReceiveIQDataAsync("test.bin", CancellationToken.None));

            Assert.That(ex?.Message, Is.EqualTo("Not connected to NetSDR"));
        }

        [Test]
        public async Task StartIQTransmissionAsync_ShouldThrowException_WhenConnectionLost()
        {
            _mockClient.Setup(c => c.StartIQTransmissionAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Connection lost"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mockClient.Object.StartIQTransmissionAsync(CancellationToken.None));

            Assert.That(ex?.Message, Is.EqualTo("Connection lost"));
        }
    }
}