using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using ClinicDentClientCommon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class PatientViewModel : BaseViewModel
    {
        IClipboardService clipboardService;
        IDialogService dialogService;

        IErrorHandler errorHandler;
        private Patient patient;
        public Patient Patient
        {
            get
            {
                return patient;
            }
            set
            {
                if (patient != value)
                {
                    patient = value;
                    NotifyPropertyChanged(nameof(Patient), nameof(PatientId), nameof(Name), nameof(Gender), nameof(Phone), nameof(Address), nameof(Birthdate), nameof(Illness), nameof(Notes), nameof(RegisterDate), nameof(CurePlan), nameof(ImageBytes));
                    SelectedStatuses = new ObservableCollection<string>(new string[] {"Новий" });
                    //SelectedStatuses = new ObservableCollection<string>(patient.Statuses?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>());
                }
            }
        }
        public int PatientId
        {
            get
            {
                return patient.Id;
            }
            set
            {
                if (patient.Id != value)
                {
                    patient.Id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get
            {
                return patient.Name;
            }
            set
            {
                if (patient.Name != value)
                {
                    patient.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Gender
        {
            get
            {
                return patient.Gender;
            }
            set
            {
                if (patient.Gender != value)
                {
                    patient.Gender = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Phone
        {
            get
            {
                return patient.Phone;
            }
            set
            {
                if (patient.Phone != value)
                {
                    patient.Phone = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Address
        {
            get
            {
                return patient.Address;
            }
            set
            {
                if (patient.Address != value)
                {
                    patient.Address = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Birthdate
        {
            get
            {
                return patient.Birthdate;
            }
            set
            {
                if (patient.Birthdate != value)
                {
                    patient.Birthdate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Illness
        {
            get
            {
                return patient.Illness;
            }
            set
            {
                if (patient.Illness != value)
                {
                    patient.Illness = value;
                    NotifyPropertyChanged();
                }

            }
        }
        public string Notes
        {
            get
            {
                return patient.Notes;
            }
            set
            {
                if (patient.Notes != value)
                {
                    patient.Notes = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string RegisterDate
        {
            get
            {
                return patient.RegisterDate;
            }
            set
            {
                if (patient.RegisterDate != value)
                {
                    patient.RegisterDate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CurePlan
        {
            get
            {
                return patient.CurePlan;
            }
            set
            {
                if (patient.CurePlan != value)
                {
                    patient.CurePlan = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public byte[] ImageBytes
        {
            get
            {
                return patient.ImageBytes;
            }
            set
            {
                if (patient.ImageBytes != value)
                {
                    patient.ImageBytes = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public AsyncCommand CopyImageCommand { get; set; }
        public AsyncCommand RemoveImageCommand { get; set; }
        public AsyncCommand PostPatientCommand { get; set; }
        public AsyncCommand PutPatientCommand { get; set; }
        public AsyncCommand RemovePatientCommand { get; set; }
        public AsyncCommand CreateOrUpdateCommand { get; set; }
        public AsyncCommand SelectPatientImageCommand { get; set; }
        public AsyncCommand NavigateToPatientDataCommand { get; set; }
        public AsyncCommand NavigateToPatientStagesCommand { get; set; }



        public PatientViewModel(IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet, IDialogService dialogServiceToSet, INavigate navigate) : base(navigate)
        {
            errorHandler = errorHandlerToSet;
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;
            CopyImageCommand = new AsyncCommand(CopyImage, null, errorHandlerToSet);
            RemoveImageCommand = new AsyncCommand(RemoveImage, null, errorHandlerToSet);
            PostPatientCommand = new AsyncCommand(PostPatient, null, errorHandlerToSet);
            PutPatientCommand = new AsyncCommand(PutPatient, null, errorHandlerToSet);
            RemovePatientCommand = new AsyncCommand(RemovePatient, null, errorHandlerToSet);
            NavigateToPatientDataCommand = new AsyncCommand(NavigateToPatientData, null, errorHandlerToSet);
            NavigateToPatientStagesCommand = new AsyncCommand(NavigateToPatientStages, null, errorHandlerToSet);

            SelectPatientImageCommand = new AsyncCommand(SelectPatientImage, null, errorHandlerToSet);
            patient = new Patient();
        }
        public async Task Initialize(Patient p)
        {
            Patient = p;
        }
        public async Task Initialize(int patientId)
        {
            Patient = await HttpService.GetPatient(patientId);
        }
        public async Task CopyImage()
        {
            if (Patient.ImageBytes != null && Patient.ImageBytes.Length > 0)
            {
                clipboardService?.CopyImage(Patient.ImageBytes);
            }
        }
        public async Task SelectPatientImage()
        {
            byte[] bytes = await dialogService?.PickFile("jpg", "jpeg", "jpe", "jfif", "png", "bmp");
            if(bytes != null)
            {
                ImageBytes = bytes;
            }
        }
        public async Task RemoveImage()
        {
            ImageBytes = null;
        }
        public async Task PostPatient()
        {
            patient.Statuses = String.Join("|", SelectedStatuses);
            patient = await HttpService.PostPatient(patient);
            NotifyPropertyChanged(nameof(PatientId));
            ViewModelStatus = ViewModelStatus.NotChanged;
        }
        public async Task PutPatient()
        {
            patient.Statuses = String.Join("|", SelectedStatuses);
            patient.LastModifiedDateTime = await HttpService.PutPatient(patient);
            ViewModelStatus = ViewModelStatus.NotChanged;
        }
        public async Task NavigateToPatientData()
        {
            await Navigation.NavigateTo($"//PatientData?patientId={PatientId}");
        }
        public async Task NavigateToPatientStages()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("PatientViewModel", this);
            await Navigation.NavigateTo($"//PatientStages", parameters);
        }
        public async Task RemovePatient()
        {
            await HttpService.DeletePatient(patient.Id);
            await Navigation.PopModal();

        }
        public async Task CreateOrUpdate()
        {
            if(ViewModelStatus == ViewModelStatus.Inserted)
            {
                PostPatientCommand.ExecuteAsync().FireAndForgetSafeAsync(errorHandler);

            }
            else
            {
                PutPatientCommand.ExecuteAsync().FireAndForgetSafeAsync(errorHandler);
            }
        }
        public string[] AllAvailableStatuses { get;} = new string[]
        {
            "Новий",
            "Запланований",
            "В роботі",
            "Виконаний",
            "Відмовився",
            "Ортодонтія"
        };
        public ObservableCollection<string> SelectedStatuses
        {
            get
            {
                return selectedStatuses;
            }
            set
            {
                if (value != selectedStatuses)
                {
                    selectedStatuses = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<string> selectedStatuses = new ObservableCollection<string>();
    }
}
