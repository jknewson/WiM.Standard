using System;

namespace WiM.Exceptions
{
    public class BadRequestException: Exception
    {
      public BadRequestException(): base() { }
    
        public BadRequestException(string message)
            : base(message) { }
    
        public BadRequestException(string format, params object[] args)
            : base(string.Format(format, args)) { }
    
        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    
        public BadRequestException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
    public class NotFoundRequestException : Exception
    {
        public NotFoundRequestException() : base() { }

        public NotFoundRequestException(string message)
            : base(message) { }

        public NotFoundRequestException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public NotFoundRequestException(string message, Exception innerException)
            : base(message, innerException) { }

        public NotFoundRequestException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
