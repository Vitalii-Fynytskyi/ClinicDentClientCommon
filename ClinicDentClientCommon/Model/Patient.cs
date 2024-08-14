namespace ClinicDentClientCommon.Model
{
    public class Patient
    {
        public static string[] AvailableSorts { get; set; } = new string[] { "За замовчуванням", "Ім'я: від А до Я", "Ім'я: від Я до А", "За замовчуванням навпаки", "Дата реєстрації: спочатку недавні", "Дата реєстрації: спочатку старіші", "Вік: спочатку молодші", "Вік: спочатку старші" };
        public static string[] AvailableStatuses { get; set; } = new string[] { "Всі статуси", "Новий", "Запланований", "В роботі", "Виконаний", "Відмовився", "Ортодонтія" };
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Birthdate { get; set; }
        public string Illness { get; set; }
        public string Notes { get; set; }
        public string RegisterDate { get; set; }
        public string Statuses { get; set; }
        public string CurePlan { get; set; }
        public byte[] ImageBytes { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastModifiedDateTime { get; set; }
    }
    public class PatientsToClient
    {
        public Patient[] Patients { get; set; }
        public int CountPages { get; set; }
    }
}
