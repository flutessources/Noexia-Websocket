namespace Noexia.Transport.WebSocket.Core.Processor
{
    public class ProcessorAdapter<T> : IProcessorAdapter where T : IMessageData
    {
        private readonly IMessageProcessor<T> _processor;

        public ProcessorAdapter(IMessageProcessor<T> processor)
        {
            _processor = processor;
        }

        public bool Process(IMessageData message, object[] objs, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _processor.Process((T)message, objs, cancellationToken);
        }
    }
}
