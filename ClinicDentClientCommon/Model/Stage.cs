using System.Collections.Generic;
using System.Linq;

namespace ClinicDentClientCommon.Model
{
    public class Stage
    {
        public Stage() { }
        public Stage(StageDTO d)
        {
            Id = d.Id;
            PatientId = d.PatientId;
            DoctorId = d.DoctorId;
            Title = d.Title;
            StageDatetime = d.StageDatetime;
            IsSentViaViber = d.IsSentViaViber;
            if (d.Operation != null)
                Operation = StageAsset.Operations.FirstOrDefault(a => a.Id == d.Operation);
            if (d.Bond != null)
                Bond = StageAsset.Bonds.FirstOrDefault(a => a.Id == d.Bond);
            if (d.Dentin != null)
                Dentin = StageAsset.Dentins.FirstOrDefault(a => a.Id == d.Dentin);
            if (d.Enamel != null)
                Enamel = StageAsset.Enamels.FirstOrDefault(a => a.Id == d.Enamel);

            if (d.CanalMethod != null)
                CanalMethod = StageAsset.CanalMethods.FirstOrDefault(a => a.Id == d.CanalMethod);
            if (d.Sealer != null)
                Sealer = StageAsset.Sealers.FirstOrDefault(a => a.Id == d.Sealer);
            if (d.Pin != null)
                Pin = StageAsset.Pins.FirstOrDefault(a => a.Id == d.Pin);
            if (d.Cement != null)
                Cement = StageAsset.Cements.FirstOrDefault(a => a.Id == d.Cement);
            if (d.Calcium != null)
                Calcium = StageAsset.Calciums.FirstOrDefault(a => a.Id == d.Calcium);
            if (d.Technician != null)
                Technician = StageAsset.Technicians.FirstOrDefault(a => a.Id == d.Technician);

            Payed = d.Payed;
            Price = d.Price;
            OldPayed = d.OldPayed;
            OldPrice = d.OldPrice;
            Expenses = d.Expenses;
            OldExpenses = d.OldExpenses;
            CommentText = d.CommentText;
            DoctorName = d.DoctorName;
            ToothUnderObservationId = d.ToothUnderObservationId;
            CreatedDateTime = d.CreatedDateTime;
            LastModifiedDateTime = d.LastModifiedDateTime;
            if(d.TeethNumbers != null)
            {
                Teeth = SharedData.AllTeeth.Where(t => d.TeethNumbers.Contains(t.Id)).ToList();
            }
            else
            {
                Teeth = new List<Tooth>();
            }
        }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public bool IsSentViaViber { get; set; }
        public string Title { get; set; }
        public string StageDatetime { get; set; }
        public StageAsset Operation { get; set; } //'Реставрація' 'Плом. каналів' 'Цементування коронок'

        //***********************FOR RESTORATION********************
        public StageAsset Bond { get; set; }
        public StageAsset Dentin { get; set; }
        public StageAsset Enamel { get; set; }

        //**********************FOR CANALS**************************
        public StageAsset CanalMethod { get; set; }
        public StageAsset Sealer { get; set; }
        public StageAsset Pin { get; set; }


        //**********************ЦЕМЕНТУВАННЯ КОРОНОК****************
        public StageAsset Cement { get; set; }
        public StageAsset Calcium { get; set; }
        public StageAsset Technician { get; set; }

        public int Payed { get; set; }
        public int Price { get; set; }
        public int OldPayed { get; set; }
        public int OldPrice { get; set; }
        public int Expenses { get; set; }
        public int OldExpenses { get; set; }
        public string CommentText { get; set; }
        public string DoctorName { get; set; }
        public int? ToothUnderObservationId { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastModifiedDateTime { get; set; }
        public List<Tooth> Teeth { get; set; } 
    }
    public class StageDTO
    {
        public StageDTO() { }
        public StageDTO(Stage d)
        {
            Id = d.Id;
            PatientId = d.PatientId;
            DoctorId = d.DoctorId;
            Title = d.Title;
            StageDatetime = d.StageDatetime;
            IsSentViaViber = d.IsSentViaViber;
            Operation = d.Operation?.Id;
            Bond = d.Bond?.Id;
            Dentin = d.Dentin?.Id;
            Enamel = d.Enamel?.Id;

            CanalMethod = d.CanalMethod?.Id;
            Sealer = d.Sealer?.Id;
            Pin = d.Pin?.Id;
            Cement = d.Cement?.Id;
            Calcium = d.Calcium?.Id;
            Technician = d.Technician?.Id;

            Payed = d.Payed;
            Price = d.Price;
            OldPayed = d.OldPayed;
            OldPrice = d.OldPrice;
            Expenses = d.Expenses;
            OldExpenses = d.OldExpenses;
            CommentText = d.CommentText;
            DoctorName = d.DoctorName;
            ToothUnderObservationId = d.ToothUnderObservationId;
            CreatedDateTime = d.CreatedDateTime;
            LastModifiedDateTime = d.LastModifiedDateTime;
            TeethNumbers = new List<byte>();
            if (d.Teeth != null)
            {
                foreach (var tooth in d.Teeth)
                {
                    TeethNumbers.Add(tooth.Id);
                }
            }
            

        }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public bool IsSentViaViber { get; set; }


        public string Title { get; set; }
        public string StageDatetime { get; set; }
        public int? Operation { get; set; } //'Реставрація' 'Плом. каналів' 'Цементування коронок'

        //***********************FOR RESTORATION********************
        public int? Bond { get; set; }
        public int? Dentin { get; set; }
        public int? Enamel { get; set; }

        //**********************FOR CANALS**************************
        public int? CanalMethod { get; set; }
        public int? Sealer { get; set; }
        public int? Pin { get; set; }


        //**********************ЦЕМЕНТУВАННЯ КОРОНОК****************
        public int? Cement { get; set; }
        public int? Calcium { get; set; }
        public int? Technician { get; set; }

        public int Payed { get; set; }
        public int Price { get; set; }
        public int OldPayed { get; set; }
        public int OldPrice { get; set; }
        public int Expenses { get; set; }
        public int OldExpenses { get; set; }

        public string CommentText { get; set; }
        public string DoctorName { get; set; }
        public int? ToothUnderObservationId { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastModifiedDateTime { get; set; }
        public List<byte> TeethNumbers { get; set; }
    }
}
