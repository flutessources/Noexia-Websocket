using Noexia.Transport.WebSocket.Core.Serialization.Binary;
using Noexia.Transport.WebSocket.Core.Serialization.Json;
using System.Reflection;

namespace Noexia.Transport.WebSocket.Core.Processor
{
    public static class MessageProcessorService
    {
        public struct MessageProcessResult
        {
            public IMessageData Message { get; set; }
            public int Id { get; set; }
            public bool Success { get; set; }
        }

        private static MessageProcessorResolver m_resolver;
        private static Dictionary<int, IDeserializerAdapter> m_deserializers = new();
        private static Dictionary<int, IProcessorAdapter> m_processors = new();
        private static Dictionary<Type, int> m_messages = new Dictionary<Type, int>();
        private static Dictionary<int, Type> m_messagesById = new Dictionary<int, Type>();
        public static IReadOnlyDictionary<int, Type> MessageById => m_messagesById;

        public static void Init(Assembly[] assemblies)
        {
            m_resolver = new MessageProcessorResolver();
            m_deserializers = m_resolver.LoadAllDeserializers(assemblies);
            m_processors = m_resolver.LoadAllProcessors(assemblies);
            m_messages = m_resolver.LoadMessageIds(assemblies);

            m_messagesById.Clear();

            foreach (var item in m_messages)
            {
                m_messagesById.Add(item.Value, item.Key);
            }
        }

        public static int GetMessageId(Type type)
        {
            if (m_messages.ContainsKey(type))
            {
                return m_messages[type];
            }
            else
            {
                return -1;
            }
        }

        public static MessageProcessResult ProcessMessage(string data, object[] objs, CancellationToken cancellationToken)
        {
            MessageContainer? container = null;

            try
            {
                container = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContainer>(data);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to deserialize the message container", ex);
            }

            if (container == null)
            {
                throw new Exception("Fail to deserialize the container");
            }

            if (container.messageData == null)
            {
                throw new Exception("Fail to deserialize the message data, object is null");
            }

            if (!m_messagesById.ContainsKey(container.messageId))
            {
                throw new Exception($"Fail to deserialize the message data, message id {container.messageId} is not registered");
            }

            Type type = null;
            if (m_messagesById.TryGetValue(container.messageId, out type) == false)
            {
                throw new Exception($"Fail to deserialize the message data, message id {container.messageId} is not registered");
            }

            string? messageDataJson = container.messageData.ToString();

            if (string.IsNullOrEmpty(messageDataJson))
            {
                throw new Exception("Fail to deserialize the message data, json is null or empty");
            }

            var messageData = Newtonsoft.Json.JsonConvert.DeserializeObject(messageDataJson, type);

            if (messageData == null)
            {
                throw new Exception("Fail to deserialize the message data");
            }

            container.messageData = messageData;

            if (m_processors.ContainsKey(container.messageId) == false)
            {
                throw new Exception($"Fail to process the message, message id {container.messageId} is not registered");
            }

            var result = m_processors[container.messageId].Process((IMessageData)container.messageData, objs, cancellationToken);

            return new MessageProcessResult()
            {
                Id = container.messageId,
                Message = (IMessageData)container.messageData,
                Success = result
            };
        }

        public static MessageProcessResult ProcessMessage(byte[] bytes, object[] objs, CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(memoryStream))
                {
                    var messageId = reader.ReadInt32();
                    var message = m_deserializers[messageId].Deserialize(reader, bytes);
                    var result = m_processors[messageId].Process(message, objs, cancellationToken);

                    return new MessageProcessResult()
                    {
                        Id = messageId,
                        Message = message,
                        Success = result
                    };
                }
            }
        }
    }
}
