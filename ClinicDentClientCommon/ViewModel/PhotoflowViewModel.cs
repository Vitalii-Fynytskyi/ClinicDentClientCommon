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
    public class PhotoflowViewModel : BaseViewModel
    {
        IClipboardService clipboardService;
        IDialogService dialogService;
        IErrorHandler errorHandler;
        IServiceProvider serviceProvider;
        public PhotoflowViewModel(IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet, IDialogService dialogServiceToSet, IServiceProvider serviceProviderToSet, INavigate navigation) : base(navigation)
        {
            serviceProvider = serviceProviderToSet;
            errorHandler = errorHandlerToSet;
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;


            PageChangedCommand = new AsyncCommand<string>(pageChanged, null, errorHandlerToSet);
            UploadImagesCommand = new AsyncCommand(uploadImages, canUploadImage, errorHandlerToSet);

            selectedPage = 1;
            visiblePages = new ObservableCollection<int>();
            DoctorViewModels = new ObservableCollection<DoctorViewModel>();
            foreach(var d in SharedData.AllDoctors)
            {
                DoctorViewModel doctorViewModel = (DoctorViewModel)serviceProviderToSet.GetService(typeof(DoctorViewModel));
                doctorViewModel.Initialize(d);
                DoctorViewModels.Add(doctorViewModel);
            }

            SelectedDoctor = DoctorViewModels.FirstOrDefault(d => d.Id == SharedData.CurrentDoctor.Id);
        }

        #region Properties
        private DoctorViewModel selectedDoctor;
        public DoctorViewModel SelectedDoctor
        {
            get
            {
                return selectedDoctor;
            }
            set
            {
                if (selectedDoctor != value)
                {
                    selectedDoctor = value;
                    SelectedPage = 1;
                    NotifyPropertyChanged(nameof(SelectedDoctor));
                    UploadImagesCommand.RaiseCanExecuteChanged();
                    ReceiveImages();
                }
            }
        }
        private ImageType imageType = ImageType.All;
        public ImageType ImageType
        {
            get
            {
                return imageType;
            }
            set
            {
                if (imageType != value)
                {
                    imageType = value;
                    SelectedPage = 1;
                    ReceiveImages();

                }
                NotifyPropertyChanged();
            }
        }
        public List<ImageType> ImageTypes { get; set; } = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().ToList();
        public bool IsAnyImageSelected
        {
            get
            {
                return imageViewModels.Any(i => i.IsSelected == true);
            }
        }
        private ObservableCollection<ImageViewModel> imageViewModels;
        public ObservableCollection<ImageViewModel> ImageViewModels
        {
            get { return imageViewModels; }
            set
            {
                if (imageViewModels != value)
                {
                    imageViewModels = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<DoctorViewModel> doctorViewModels;
        public ObservableCollection<DoctorViewModel> DoctorViewModels
        {
            get
            {
                return doctorViewModels;
            }
            set
            {
                if (doctorViewModels != value)
                {
                    doctorViewModels = value;
                }
            }
        }
        private ObservableCollection<int> visiblePages;
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
        #endregion
        #region Commands
        public AsyncCommand<string> PageChangedCommand { get; set; }
        public AsyncCommand UploadImagesCommand { get; set; }

        private async Task pageChanged(string pageNumber)
        {
            SelectedPage = Convert.ToInt32(pageNumber);
            ReceiveImages();
        }
        private async Task uploadImages()
        {

        }
        private bool canUploadImage()
        {
            return SelectedDoctor.Id == SharedData.CurrentDoctor.Id;
        }
        #endregion

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
        public async Task ReceiveImages()
        {
            ImagesToClient imagesToClient = new ImagesToClient();
            try
            {
                imagesToClient = await HttpService.GetImages(selectedPage, SharedData.Settings.PhotosPerPage, SelectedDoctor.Id, imageType);
            }
            catch (Exception ex)
            {
                dialogService?.DisplayMessage($"Не вдалось завантажити зображення: {ex.Message}");
                return;
            }
            CountPages = imagesToClient.CountPages;
            ImageViewModels = new ObservableCollection<ImageViewModel>();
            foreach (var i in imagesToClient.Images)
            {
                ImageViewModel imageViewModel = (ImageViewModel)serviceProvider.GetService(typeof(ImageViewModel));
                imageViewModel.Initialize(i, photoflowViewModelToSet: this);
                ImageViewModels.Add(imageViewModel);
            }
            createVisiblePages();
        }
    }
}
