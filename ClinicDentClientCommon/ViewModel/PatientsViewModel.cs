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
    public enum PatientListMode
    {
        MyPatients, AllPatients, SearchPatientsWithImage
    }
    public class PatientsViewModel :BaseViewModel
    {
        private IClipboardService clipboardService;
        private IDialogService dialogService;
        private IErrorHandler errorHandler;
        public Doctor SelectedDoctor
        {
            get
            {
                return selectedDoctor;
            }
            set
            {
                if (value != selectedDoctor)
                {
                    selectedDoctor = value;
                    NotifyPropertyChanged();
                    SelectedPage = 1;
                    ReceivePatients();
                }
            }
        }
        private Doctor selectedDoctor = null;

        public PatientListMode PatientListMode
        {
            get { return patientListMode; }
            set
            {
                if (patientListMode != value)
                {
                    patientListMode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PatientListMode patientListMode;

        public ObservableCollection<int> VisiblePages
        {
            get
            {
                return visiblePages;
            }
            set
            {
                if (visiblePages != value)
                {
                    visiblePages = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<int> visiblePages;

        public ObservableCollection<PatientViewModel> PatientViewModels
        {
            get { return patientViewModels; }
            set
            {
                if (patientViewModels != value)
                {
                    patientViewModels = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<PatientViewModel> patientViewModels;

        private int? imageId;


        public PatientsViewModel(INavigate navigation, IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet,IDialogService dialogServiceToSet):base(navigation)
        {
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;
            errorHandler = errorHandlerToSet;
            SearchPressedCommand = new AsyncCommand(SearchPressed,null,errorHandlerToSet);
            PageChangedCommand = new AsyncCommand<string>(PageChanged, null, errorHandlerToSet);
        }
        public async Task Initialize(PatientListMode patientListModeToSet, int? imageIdToSet = null)
        {
            PatientListMode = patientListModeToSet;
            selectedDoctor = SharedData.AllDoctors.FirstOrDefault(d => d.Id == SharedData.CurrentDoctor.Id);
            NotifyPropertyChanged(nameof(SelectedDoctor));
            imageId = imageIdToSet;
            selectedSorting = Patient.AvailableSorts[0];
            NotifyPropertyChanged(nameof(SelectedSorting));

            selectedStatus = Patient.AvailableStatuses[0];
            NotifyPropertyChanged(nameof(SelectedStatus));

            selectedPage = 1;
            searchText = string.Empty;
            visiblePages = new ObservableCollection<int>();
            ReceivePatients();
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string selectedSorting;
        public string SelectedSorting
        {
            get { return selectedSorting; }
            set
            {
                if (selectedSorting != value)
                {
                    selectedSorting = value;
                    NotifyPropertyChanged();
                    SelectedPage = 1;
                    ReceivePatients();
                }
            }
        }
        private int countPages;
        public int CountPages
        {
            get { return countPages; }
            set
            {
                if (countPages != value)
                {
                    countPages = value;
                    if (SelectedPage > CountPages)
                    {
                        SelectedPage = CountPages;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private int selectedPage;
        public int SelectedPage
        {
            get { return selectedPage; }
            set
            {
                if (selectedPage != value)
                {
                    selectedPage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string selectedStatus;
        public string SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                if (selectedStatus != value)
                {
                    selectedStatus = value;
                    NotifyPropertyChanged();
                    SelectedPage = 1;
                    ReceivePatients();
                }
            }
        }
        public AsyncCommand SearchPressedCommand { get; set; }
        public async Task SearchPressed()
        {
            SelectedPage = 1;
            ReceivePatients();
        }
        public AsyncCommand<string> PageChangedCommand { get; set; }
        public async Task PageChanged(string page)
        {
            SelectedPage = Convert.ToInt32(page);
            ReceivePatients();
        }
        public async Task ReceivePatients()
        {
            string searchTextForRequest = SearchText == "" ? "<null>" : SearchText;
            PatientsToClient patientsToClient = null;
            try
            {
                switch (PatientListMode)
                {
                    case PatientListMode.AllPatients:
                        patientsToClient = await HttpService.GetPatients(selectedStatus, selectedSorting, selectedPage, SharedData.Settings.PatientsPerPage, searchTextForRequest);
                        break;
                    case PatientListMode.MyPatients:
                        patientsToClient = await HttpService.GetPatients(selectedStatus, selectedSorting, selectedPage, SharedData.Settings.PatientsPerPage, searchTextForRequest, SelectedDoctor.Id);
                        break;
                    case PatientListMode.SearchPatientsWithImage:
                        patientsToClient = await HttpService.GetPatientsByImage(imageId.Value, selectedStatus, selectedSorting, selectedPage, SharedData.Settings.PatientsPerPage, searchTextForRequest);
                        break;
                }
            }
            catch (Exception ex)
            {
                errorHandler?.HandleError(ex);
            }
            CountPages = patientsToClient.CountPages;
            PatientViewModels = new ObservableCollection<PatientViewModel>(patientsToClient.Patients.Select((p) => 
            {
                var patientViewModel = new PatientViewModel(clipboardService, errorHandler, dialogService, Navigation);
                patientViewModel.Initialize(p);
                return patientViewModel;
            }));
            createVisiblePages();

        }
        private void createVisiblePages()
        {
            if (CountPages <= 1)
            {
                VisiblePages.Clear();
                return;
            }
            List<int> pages = new List<int>(15);
            int pageToAdd = SelectedPage;
            pages.Add(pageToAdd);
            pageToAdd--;
            while (pageToAdd > 0 && pageToAdd >= SelectedPage - 7)
            {
                pages.Insert(0, pageToAdd);
                pageToAdd--;
            }
            pageToAdd = SelectedPage + 1;
            while (CountPages >= pageToAdd && pageToAdd <= SelectedPage + 7)
            {
                pages.Add(pageToAdd);
                pageToAdd++;
            }
            VisiblePages = new ObservableCollection<int>(pages);
        }
    }
}
