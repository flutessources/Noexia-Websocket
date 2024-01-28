using NUnit.Framework;
using Moq;
using System.Threading;
using Noexia.Transport.WebSocket.Core.Processor;
using Noexia.Transport.WebSocket.Core.Test.Messages;
// Autres espaces de noms nécessaires

[TestFixture]
public class ProcessorAdapterTests
{
    private Mock<IMessageProcessor<Message1>> _mockProcessor;
    private ProcessorAdapter<Message1> _adapter;

    [SetUp]
    public void Setup()
    {
        _mockProcessor = new Mock<IMessageProcessor<Message1>>();
        _adapter = new ProcessorAdapter<Message1>(_mockProcessor.Object);
    }

    [Test]
    public void Process_ShouldCallUnderlyingProcessor()
    {
        // Arrange
        var message = new Message1();
        var objs = new object[] { };
        var cancellationToken = CancellationToken.None;

        // Act
        _adapter.Process(message, objs, cancellationToken);

        // Assert
        _mockProcessor.Verify(p => p.Process(message, objs, cancellationToken), Times.Once);
    }

    // Ajoutez d'autres tests pour différents scénarios et cas d'erreur
    [Test]
    public void Process_WithNullMessage_ShouldThrowArgumentNullException()
    {
        // Arrange
        Message1 message = null;
        var objs = new object[] { };
        var cancellationToken = CancellationToken.None;

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() => _adapter.Process(message, objs, cancellationToken));
    }
}