using Noexia.Transport.WebSocket.Core.Processor;
using Noexia.Transport.WebSocket.Core.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.Transport.WebSocket.Core.Test.Processor
{
    internal class ProcessorMessage1 : IMessageProcessor<Message1>
    {
        public bool Process(Message1 message, object[] objs, CancellationToken cancellationToken = default)
        {
            return true;
        }
    }
}
