using NUnit.Framework;
using System.Reflection;
using Noexia.Transport.WebSocket.Core.Processor;
using Noexia.Transport.WebSocket.Core.Test.Messages;
using Moq;
// Assurez-vous d'inclure les espaces de noms appropriés pour vos classes et attributs

[TestFixture]
public class MessageProcessorResolverTests
{
    private MessageProcessorResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _resolver = new MessageProcessorResolver();
    }

    [Test]
    public void LoadMessageIds_WithValidAssemblies_ShouldReturnCorrectIds()
    {
        // Arrange
        var assemblies = new[] { Assembly.GetExecutingAssembly() }; // Remplacez par vos assemblages

        // Act
        var result = _resolver.LoadMessageIds(assemblies);

        // Assert
        // Remplacez 'YourMessageDataClass' et 'expectedId' par vos valeurs réelles
        Assert.IsTrue(result.ContainsKey(typeof(Message1)));
        Assert.AreEqual(1, result[typeof(Message1)]);
    }

    // Ajoutez d'autres tests pour différents scénarios et cas d'erreur
    [Test]
    public void LoadMessageIds_WithNullAssemblies_ShouldThrowArgumentNullException()
    {
        // Arrange
        Assembly[] assemblies = null;

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() => _resolver.LoadMessageIds(assemblies));
    }

    [Test]
    public void LoadMessageIds_WithEmptyAssemblies_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var assemblies = new Assembly[] { };

        // Act
        var result = _resolver.LoadMessageIds(assemblies);

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void LoadMessageIds_WithAssembliesWithoutMessageData_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var assemblies = new[] { Assembly.GetExecutingAssembly() }; // Remplacez par vos assemblages

        // Act
        var result = _resolver.LoadMessageIds(assemblies);

        // Assert
        // Remplacez 'YourMessageDataClass' par votre classe de données de message
        Assert.IsFalse(result.ContainsKey(typeof(MessageWithoutMessageData)));
    }
}