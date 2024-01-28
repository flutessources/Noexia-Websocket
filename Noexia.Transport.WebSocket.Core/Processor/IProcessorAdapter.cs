namespace Noexia.Transport.WebSocket.Core.Processor
{
    public interface IProcessorAdapter
    {
        bool Process(IMessageData message, object[] objs, CancellationToken cancellationToken);
    }
}
