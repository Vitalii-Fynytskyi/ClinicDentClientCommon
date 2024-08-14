namespace ClinicDentClientCommon.Model
{
    public class PatientWithDebt : Patient
    {
        public static string[] DebtorsAvailableSorts { get; set; } = new string[] { "За замовчуванням", "Ім'я: від А до Я", "Ім'я: від Я до А", "За замовчуванням навпаки", "Дата реєстрації: спочатку недавні", "Дата реєстрації: спочатку старіші", "Вік: спочатку молодші", "Вік: спочатку старші", "Сума боргу: спочатку більші", "Сума боргу: спочатку менші" };
        public int DebtSum { get; set; }
    }
    public class DebtPatientsToClient
    {
        public PatientWithDebt[] Patients { get; set; }
        public int CountPages { get; set; }
    }
}
