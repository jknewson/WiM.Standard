using System.Collections.Generic;
using System.Collections;
namespace WiM.Resources
{
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
