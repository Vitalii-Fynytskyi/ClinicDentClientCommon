namespace ClinicDentClientCommon.Model
{
    public class ToothUnderObservation
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public int StageId { get; set; }
        public string ToothName { get; set; }
        public string ObservationDescription { get; set; }
    }
}
