using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.Transport.WebSocket.Core.Test.Messages
{
    [Message(1)]
    public class Message1 : IMessageData
    {
        public string StringTest { get; set; }
        public int IntTest { get; set; }
    }
}
