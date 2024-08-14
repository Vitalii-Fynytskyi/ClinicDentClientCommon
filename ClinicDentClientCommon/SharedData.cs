using ClinicDentClientCommon.Model;
using System;

namespace ClinicDentClientCommon
{
    public static class SharedData
    {
        public static DateTime? GetDateTimeFromString(string datetime)
        {
            DateTime dt;
            if (DateTime.TryParse(datetime, out dt) == false)
            {
                return null;
            }
            return dt;
        }
        public static Doctor CurrentDoctor { get; set; }
        public static Doctor[] AllDoctors { get; set; }
        public static Cabinet[] AllCabinets { get; set; }
        public static string DateTimePattern { get; set; } = "yyyy-MM-dd HH:mm";
        public static string DatePattern { get; set; } = "dd.MM.yyyy";
        public static string ExactDateTimePattern { get; set; } = "yyyy-MM-dd HH:mm:ss.fffffff";
    }
}
