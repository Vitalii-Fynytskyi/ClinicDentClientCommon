using ClinicDentClientCommon.Model;
using System;
using System.Collections.Generic;

namespace ClinicDentClientCommon
{
    public static class SharedData
    {
        static SharedData()
        {
            AllTeeth = new List<Tooth>(59);
            for(byte i = 11; i <= 48; i++)
            {
                AllTeeth.Add(new Tooth(i));
            }
            AllTeeth.Add(new Tooth(51,true));
            AllTeeth.Add(new Tooth(52, true));
            AllTeeth.Add(new Tooth(53, true));
            AllTeeth.Add(new Tooth(54, true));
            AllTeeth.Add(new Tooth(55, true));

            AllTeeth.Add(new Tooth(61, true));
            AllTeeth.Add(new Tooth(62, true));
            AllTeeth.Add(new Tooth(63, true));
            AllTeeth.Add(new Tooth(64, true));
            AllTeeth.Add(new Tooth(65, true));

            AllTeeth.Add(new Tooth(71, true));
            AllTeeth.Add(new Tooth(72, true));
            AllTeeth.Add(new Tooth(73, true));
            AllTeeth.Add(new Tooth(74, true));
            AllTeeth.Add(new Tooth(75, true));

            AllTeeth.Add(new Tooth(81, true));
            AllTeeth.Add(new Tooth(82, true));
            AllTeeth.Add(new Tooth(83, true));
            AllTeeth.Add(new Tooth(84, true));
            AllTeeth.Add(new Tooth(85, true));


        }
        public static List<Tooth> AllTeeth { get; set; }
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
        public static Settings Settings { get; set; }
        public static Cabinet DefaultSelectedCabinet { get; set; }
    }
}
