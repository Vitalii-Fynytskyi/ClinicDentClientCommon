using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class DoctorViewModel : BaseViewModel
    {
        private Doctor doctor;
        public Doctor Doctor
        {
            get
            {
                return doctor;
            }
            set
            {
                if(value != doctor)
                {
                    doctor = value;
                    NotifyPropertyChanged(nameof(Doctor), nameof(Id), nameof(Name));
                }
            }
        }

        public DoctorViewModel(INavigate navigation):base(navigation)
        {
        }
        public async Task Initialize(Doctor doctorToSet)
        {
            Doctor = doctorToSet;
        }
        public int Id
        {
            get
            {
                return doctor.Id;
            }
            set
            {
                if (doctor.Id != value)
                {
                    doctor.Id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get
            {
                return doctor.Name;
            }
            set
            {
                if (doctor.Name != value)
                {
                    doctor.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
