using System.Collections.Generic;

namespace ClinicDentClientCommon.RequestAnswers
{
    public class PutStagesRequestAnswer
    {
        public List<int> ConflictedStagesIds { get; set; }
        public string NewLastModifiedDateTime { get; set; }
    }
}
