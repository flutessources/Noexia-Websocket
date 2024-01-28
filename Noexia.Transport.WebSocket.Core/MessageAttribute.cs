namespace Noexia.Transport.WebSocket.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public readonly ushort Id;
        public MessageAttribute(ushort id)
        {
            Id = id;
        }
    }
}
