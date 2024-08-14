using System;

namespace ClinicDentClientCommon.Exceptions
{
    public class ConflictException : Exception
    {
        public object Param;
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, object param) : base(message)
        {
            Param = param;
        }
    }
}
