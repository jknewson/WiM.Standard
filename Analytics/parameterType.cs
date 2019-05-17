using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Services.Analytics
{
    public class parameterType
    {
        public int Value { get; set; }

        public const int operation = 1;
        public const int path = 2;
        public const int resource = 3;
        public const int item = 4;
        public const int action = 5;
        public const int queryparams = 6;
        public const int referrer_ip_address = 7;
        public const int serviceHost = 8;

        public parameterType(int value)
        {
            Value = value;
        }

        public static implicit operator int(parameterType cType)
        {
            return cType.Value;
        }

        public static implicit operator parameterType(int value)
        {
            return new parameterType(value);
        }
    }
}
