using System;

namespace ClinicDentClientCommon.Interfaces
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
