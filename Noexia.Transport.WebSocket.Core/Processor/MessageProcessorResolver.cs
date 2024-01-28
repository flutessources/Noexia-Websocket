using System.Reflection;
using Noexia.Transport.WebSocket.Core.Serialization.Binary;

namespace Noexia.Transport.WebSocket.Core.Processor
{
    public class MessageProcessorResolver
    {
        public MessageProcessorResolver()
        {

        }

        public Dictionary<Type, int> LoadMessageIds(Assembly[] assemblies)
        {
            Dictionary<Type, int> ids = new Dictionary<Type, int>();
            var types = assemblies.SelectMany(x => x.GetTypes()).Where(x => x.GetInterfaces().Any(y => y == typeof(IMessageData)));
            foreach (var type in types)
            {
                var attributes = type.CustomAttributes.Where(x => x.AttributeType == typeof(MessageAttribute));
                if (attributes == null || attributes.Count() == 0)
                {
                    Console.WriteLine($"Warning : fail to parse the attribute MessageAttribute for type {type}");
                    continue;
                }

                var attribute = attributes.First();
                if (attribute.ConstructorArguments.Count == 0)
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : attribute has not constructor arguments");
                }

                var firstArg = attribute.ConstructorArguments[0].Value;

                if (firstArg == null)
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : imposible to find first argument of attribute constructor");
                }

                ushort id = 0;
                if (ushort.TryParse(firstArg.ToString(), out id))
                {
                    ids.Add(type, id);
                }
                else
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : the first argument of attribute is not castable to ushort");
                }
            }

            return ids;
        }


        public Dictionary<int, IDeserializerAdapter> LoadAllDeserializers(Assembly[] assemblies)
        {
            var res = new Dictionary<int, IDeserializerAdapter>();

            var types = assemblies.SelectMany(x => x.GetTypes())
                                  .Where(x => x.GetInterfaces()
                                  .Any(y => y.IsGenericType &&
                                    y.GetGenericTypeDefinition() == typeof(IDeserializableMessage<>)))
                                  .ToList();

            foreach (var type in types)
            {
                var interfaceType = type.GetInterfaces().First(x => x.IsGenericType &&
                                      x.GetGenericTypeDefinition() == typeof(IDeserializableMessage<>));
                var messageType = interfaceType.GetGenericArguments()[0];
                var attribute = messageType.GetCustomAttribute<MessageAttribute>();

                if (attribute == null)
                {
                    Console.WriteLine($"Warning: fail to parse the attribute MessageAttribute for {messageType.Name}");
                    continue;
                }

                var id = (int)attribute.Id; // Assumant que l'Id est accessible directement.
                var deserializerInstance = Activator.CreateInstance(type);

                if (deserializerInstance == null)
                {
                   throw new Exception($"Fail to create instance of {type.Name}");
                }

                var adapterType = typeof(DeserializerAdapter<>).MakeGenericType(new Type[] { messageType });
                var adapterInstance = Activator.CreateInstance(adapterType, new object[] { deserializerInstance });

                if (adapterInstance == null)
                {
                    throw new Exception($"Fail to create instance of {adapterType.Name}");
                }

                IDeserializerAdapter? adapter = adapterInstance as IDeserializerAdapter;
                if (adapter == null)
                {
                    throw new Exception($"Fail to cast {adapterInstance.GetType().Name} to IDeserializerAdapter");
                }

                res.Add(id, adapter);
            }

            return res;
        }

        public Dictionary<int, IProcessorAdapter> LoadAllProcessors(Assembly[] assemblies)
        {
            Dictionary<int, IProcessorAdapter> res = new();

            var types = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.GetInterfaces()
                .Any(y => y.IsGenericType &&
                    y.GetGenericTypeDefinition() == typeof(IMessageProcessor<>)))
                .ToList();

            foreach (var type in types)
            {
                var interfaceType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMessageProcessor<>));
                var messageType = interfaceType.GetGenericArguments()[0];

                var attribute = messageType.CustomAttributes.First(x => x.AttributeType == typeof(MessageAttribute));
                if (attribute == null)
                {
                    Console.WriteLine("Warning : fail to parse the attribute MessageAttribute");
                    continue;
                }

                if (attribute.ConstructorArguments.Count == 0)
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : attribute has not constructor arguments");
                }

                var firstArg = attribute.ConstructorArguments[0].Value;
                if (firstArg == null)
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : imposible to find first argument of attribute constructor");
                }

                ushort id = 0;
                if (ushort.TryParse(firstArg.ToString(), out id) == false)
                {
                    throw new Exception("Fail to parse the attribute MessageAttribute : the first argument of attribute is not castable to ushort");
                }

                var deserializerInstance = Activator.CreateInstance(type);

                if (deserializerInstance == null)
                {
                    throw new Exception($"Fail to create instance of {type.Name}");
                }

                var adapterType = typeof(ProcessorAdapter<>).MakeGenericType(new Type[] { messageType });
                var adapterInstance = Activator.CreateInstance(adapterType, new object[] { deserializerInstance });

                IProcessorAdapter? adapter = adapterInstance as IProcessorAdapter;
                if (adapter == null)
                {
                    throw new Exception($"Fail to cast {deserializerInstance.GetType().Name} to IProcessorAdapter");
                }


                res.Add(id, adapter);
            }
            return res;
        }
    }
}
