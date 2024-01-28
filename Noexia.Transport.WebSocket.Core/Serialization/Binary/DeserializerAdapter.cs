namespace Noexia.Transport.WebSocket.Core.Serialization.Binary
{
    public class DeserializerAdapter<T> : IDeserializerAdapter where T : IMessageData
    {
        private readonly IDeserializableMessage<T> _deserializer;

        public DeserializerAdapter(IDeserializableMessage<T> deserializer)
        {
            _deserializer = deserializer;
        }

        public IMessageData Deserialize(BinaryReader reader, byte[] data)
        {
            return _deserializer.Deserialize(reader, data);
        }
    }
}