using NUnit.Framework;
using Moq;
using System.Threading;
using Noexia.Transport.WebSocket.Core.Processor;
using System.Reflection;
using Noexia.Transport.WebSocket.Core.Test.Messages;
// Autres espaces de noms nécessaires

[TestFixture]
public class MessageProcessorServiceTests
{
    [SetUp]
    public void Setup()
    {
        // Initialisation avec les assemblées nécessaires
        Assembly[] assemblies = { typeof(Message1).Assembly };
        MessageProcessorService.Init(assemblies);
    }

    [Test]
    public void ProcessMessage_WithValidJsonData_ShouldProcessSuccessfully()
    {
        // Arrange
        string jsonData = "{\r\n    \"messageId\" : 1,\r\n    \"messageData\" : \r\n    {\r\n        \"StringTest\" : \"Hello World\",\r\n\t\"IntTest\" : 2\r\n    }\r\n}";
        object[] objs = new object[] { /* Dépendances, si nécessaire */ };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = MessageProcessorService.ProcessMessage(jsonData, objs, cancellationToken);

        // Assert
        Assert.IsTrue(result.Success);
        // Autres assertions...
    }

    [Test]
    public void ProcessMessage_WithInvalidJsonData_ShouldThrowException()
    {
        // Arrange
        string jsonData = "Invalid JSON data";
        object[] objs = new object[] { /* Dépendances, si nécessaire */ };
        var cancellationToken = CancellationToken.None;

        // Act
        // Assert
        Assert.Throws<Exception>(() => MessageProcessorService.ProcessMessage(jsonData, objs, cancellationToken));
    }

    [Test]
    public void ProcessMessage_WithNullJsonData_ShouldThrowException()
    {
        // Arrange
        string jsonData = null;
        object[] objs = new object[] { /* Dépendances, si nécessaire */ };
        var cancellationToken = CancellationToken.None;

        // Act
        // Assert
        Assert.Throws<Exception>(() => MessageProcessorService.ProcessMessage(jsonData, objs, cancellationToken));
    }

    [Test]
    public void ProcessMessage_WithEmptyJsonData_ShouldThrowException()
    {
        // Arrange
        string jsonData = "";
        object[] objs = new object[] { /* Dépendances, si nécessaire */ };
        var cancellationToken = CancellationToken.None;

        // Act
        // Assert
        Assert.Throws<Exception>(() => MessageProcessorService.ProcessMessage(jsonData, objs, cancellationToken));
    }

    [Test]
    public void ProcessMessage_WithInvalidMessageId_ShouldThrowException()
    {
        // Arrange
        string jsonData = "{\r\n    \"messageId\" : 999,\r\n    \"messageData\" : \r\n    {\r\n        \"StringTest\" : \"Hello World\",\r\n\t\"IntTest\" : 2\r\n    }\r\n}";
        object[] objs = new object[] { /* Dépendances, si nécessaire */ };
        var cancellationToken = CancellationToken.None;

        // Act
        // Assert
        Assert.Throws<Exception>(() => MessageProcessorService.ProcessMessage(jsonData, objs, cancellationToken));
    }
}