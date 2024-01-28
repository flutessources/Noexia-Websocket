namespace Noexia.Transport.WebSocket.Core.Serialization.Binary
{
    public interface IDeserializableMessage<out T> where T : IMessageData
    {
        public T Deserialize(BinaryReader reader, byte[] data);
    }
}
