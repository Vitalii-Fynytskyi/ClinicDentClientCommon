using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Exceptions;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using ClinicDentClientCommon.RequestAnswers;
using ClinicDentClientCommon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class StagesViewModel : BaseViewModel
    {
        IClipboardService clipboardService;
        IDialogService dialogService;
        IErrorHandler errorHandler;
        IServiceProvider serviceProvider;

        public StagesViewModel(IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet, IDialogService dialogServiceToSet, IServiceProvider serviceProviderToSet, INavigate navigate) : base(navigate)
        {
            serviceProvider = serviceProviderToSet;
            errorHandler = errorHandlerToSet;
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;
            PhotoClickedCommand = new AsyncCommand(PhotoClicked,null, errorHandlerToSet);
            EditPatientCommand = new AsyncCommand(EditPatient,null, errorHandlerToSet);
            UpdateCurePlanCommand = new AsyncCommand<object>(UpdateCurePlan, null, errorHandlerToSet);
            CreateNewStageCommand = new AsyncCommand<string>(CreateNewStage, null, errorHandlerToSet);
        }
        public async Task Initialize(int patientId, DateTime? date = null)
        {

        }
        public async Task Initialize(PatientViewModel vm)
        {
            await LoadAllPatientStages(vm);
        }
        #region Properties
        public ObservableCollection<ScheduleViewModel> FutureAppointmentsViewModels
        {
            get
            {
                return futureAppointmentsViewModels;
            }
            set
            {
                if (futureAppointmentsViewModels != value)
                {
                    futureAppointmentsViewModels = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<ScheduleViewModel> futureAppointmentsViewModels;
        public DateTime? MarkedDate { get; set; } = null;
        public string LastHygieneText
        {
            get
            {
                return lastHygieneText;
            }
            set
            {
                if (value != lastHygieneText)
                {
                    lastHygieneText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string lastHygieneText;
        PatientViewModel patient;
        public PatientViewModel Patient
        {
            get
            {
                return patient;
            }
            set
            {
                if (value != patient)
                {
                    patient = value;
                    NotifyPropertyChanged(nameof(Patient), nameof(CurePlan));
                }

            }
        }
        public string CurePlan
        {
            get
            {
                return Patient?.CurePlan;
            }
            set
            {
                if (value != Patient.CurePlan)
                {
                    Patient.CurePlan = value;
                    Mark_IsCurePlanUpdated = true;
                }
            }
        }
        ObservableCollection<StageViewModel> stages;
        public ObservableCollection<StageViewModel> Stages
        {
            get
            {
                return stages;
            }
            set
            {
                if (value != stages)
                {
                    stages = value;
                    UpdateHygienaState();
                    NotifyPropertyChanged();

                }
            }
        }
        public bool Mark_IsCurePlanUpdated = false;

        #endregion
        #region Commands
        public AsyncCommand PhotoClickedCommand { get; set; }
        public AsyncCommand EditPatientCommand { get; set; }
        public AsyncCommand<object> UpdateCurePlanCommand { get; set; }
        public AsyncCommand<string> CreateNewStageCommand { get; set; }


        private async Task PhotoClicked()
        {
        }
        private async Task EditPatient()
        {
        }
        private async Task UpdateCurePlan(object arg)
        {
        }
        private async Task UpdateCurePlanInternal(object arg)
        {
        }
        private bool CanUpdateCurePlan(object arg)
        {
            return Mark_IsCurePlanUpdated;
        }
        public async Task<OperationResult> ServerUpdateStages()
        {
            return null;
        }
        public void ReloadStages()
        {
            if (MarkedDate != null)
                LoadAllPatientStagesWithRelatedMarked(MarkedDate.Value, Patient.PatientId);
            else
                LoadAllPatientStages(Patient);
        }
        public void UpdateHygienaState()
        {
            
        }
        private void UpdateViewModels(List<StageViewModel> viewModels, PutStagesRequestAnswer response)
        {
            

        }
        private void UpdateViewModel(StageViewModel viewModel, string lastModifiedDateTime)
        {
            viewModel.ViewModelStatus = ViewModelStatus.NotChanged;
            viewModel.Stage.OldPrice = viewModel.Stage.Price;
            viewModel.Stage.OldPayed = viewModel.Stage.Payed;
            viewModel.Stage.OldExpenses = viewModel.Stage.Expenses;
            viewModel.Stage.LastModifiedDateTime = lastModifiedDateTime;
        }
        private void HandleConflictException(List<StageViewModel> viewModels, ConflictException ex, OperationResult operationResult)
        {
        }
        private async Task CreateNewStage(string stageName)
        {
        }
        #endregion
        public async Task LoadAllPatientStages(PatientViewModel patientToSet)
        {
            Patient = patientToSet;
            Stages = new ObservableCollection<StageViewModel>();
            List<Stage> stages = await HttpService.GetPatientStages(patientToSet.PatientId);
            foreach (Stage stage in stages)
            {
                StageViewModel stageViewModel = (StageViewModel)serviceProvider.GetService(typeof(StageViewModel));
                await stageViewModel.Initialize(stage, this);
                Stages.Add(stageViewModel);
            }
        }
        public async Task LoadAllPatientStagesWithRelatedMarked(DateTime markedDateToSet, int patientId)
        {
            MarkedDate = markedDateToSet;
            Patient loadedPatient = await HttpService.GetPatient(patientId);
            Patient = (PatientViewModel)serviceProvider.GetService(typeof(PatientViewModel));
            await Patient.Initialize(loadedPatient);
            Stages = new ObservableCollection<StageViewModel>();
            List<Stage> stages = await HttpService.GetPatientStages(patientId);
            foreach (Stage stage in stages)
            {
                StageViewModel stageViewModel = (StageViewModel)serviceProvider.GetService(typeof(StageViewModel));
                await stageViewModel.Initialize(stage, this);
                Stages.Add(stageViewModel);
            }
        }
    }
}
