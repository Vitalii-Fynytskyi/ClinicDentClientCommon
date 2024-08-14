using System;

namespace ClinicDentClientCommon.Exceptions
{
    public class NotFoundException : Exception
    {
        public object Param;
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, object param) : base(message)
        {
            Param = param;
        }
    }
}
