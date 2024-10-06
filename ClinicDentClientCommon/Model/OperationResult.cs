using System;
using System.Collections.Generic;

namespace ClinicDentClientCommon.Model
{
    public class OperationResult
    {
        public bool AllowDeactivateWindow { get; set; } = true;
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
    }
}
