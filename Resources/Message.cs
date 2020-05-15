using System.Collections.Generic;

namespace WIM.Resources
{
    public interface IMessage
    {
        void sm(Message msg);
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
