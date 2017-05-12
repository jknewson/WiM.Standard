using System.Collections.Generic;

namespace WiM.Resources
{
    public interface IMessage
    {
        List<Message> Messages { get; }
    }
    public struct Message
    {
        public MessageType type { get; set; }
        public string msg { get; set; }
    }
    public enum MessageType
    {
        info,
        warning,
        error
    }
}
