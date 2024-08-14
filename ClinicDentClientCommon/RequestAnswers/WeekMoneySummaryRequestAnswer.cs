using System.Collections.Generic;

namespace ClinicDentClientCommon.RequestAnswers
{
    public class WeekMoneySummaryRequestAnswer
    {
        public List<int> StagesPaidSum { get; set; }
        public List<int> StagesPriceSum { get; set; }
        public List<int> StagesExpensesSum { get; set; }
        public List<int> DoctorIds { get; set; }

    }
}
