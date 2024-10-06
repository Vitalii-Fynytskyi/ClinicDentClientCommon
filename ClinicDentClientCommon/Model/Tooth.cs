using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicDentClientCommon.Model
{
    public class Tooth
    {
        public Tooth() { }
        public Tooth(byte toothNumber, bool isBabyTooth = false)
        {
            Id = toothNumber;
            IsBabyTooth=isBabyTooth;
        }
        public byte Id { get; set; }
        public bool IsBabyTooth { get; set; }
    }
}
