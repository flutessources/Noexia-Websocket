namespace Noexia.Transport.WebSocket.Core.Serialization.Binary
{
    public interface IDeserializerAdapter
    {
        IMessageData Deserialize(BinaryReader reader, byte[] data);
    }
}
