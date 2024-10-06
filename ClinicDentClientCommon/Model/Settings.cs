namespace ClinicDentClientCommon.Model
{
    public class Settings
    {
        public bool CanDeleteImage { get; set; }
        public int PatientsPerPage { get; set; }
        public int PhotosPerPage { get; set; }
        public string ServerAddress { get; set; }
        public string LanServerAddress { get; set; }

        public int DefaultSelectedTable { get; set; }
        public string DefaultTenant { get; set; }
        public int TcpPort { get; set; }
        public string Telegram_PhoneNumber { get; set; }
        public string Telegram_OptionalPassword { get; set; }
        public int Telegram_ApiId { get; set; }
        public string Telegram_ApiHash { get; set; }
    }
}
