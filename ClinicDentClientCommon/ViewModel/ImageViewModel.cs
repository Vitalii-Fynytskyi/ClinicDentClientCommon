using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using System.Threading.Tasks;
namespace ClinicDentClientCommon.ViewModel
{
    public class ImageViewModel : BaseViewModel
    {
        IClipboardService clipboardService;
        IDialogService dialogService;
        IErrorHandler errorHandler;
        public ImageViewModel(IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet, IDialogService dialogServiceToSet, INavigate navigation) : base(navigation)
        {
            errorHandler = errorHandlerToSet;
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;

            ImageClickedCommand = new AsyncCommand(ImageClicked, null, errorHandlerToSet);
            CopyImageCommand = new AsyncCommand(CopyImage, null, errorHandlerToSet);
            DeleteImageCommand = new AsyncCommand(DeleteImage, null, errorHandlerToSet);
            DeleteImageFromStageCommand = new AsyncCommand(DeleteImageFromStage, null, errorHandlerToSet);
            RenameImageCommand = new AsyncCommand(RenameImage, null, errorHandlerToSet);
            ShowReferencesCommand = new AsyncCommand(ShowReferencesToPatients, null, errorHandlerToSet);

        }
        public async Task Initialize(Image imageToSet, StageViewModel ownerToSet = null, PhotoflowViewModel photoflowViewModelToSet = null)
        {
            ownerStage = ownerToSet;
            photoflowOwner = photoflowViewModelToSet;
            image = imageToSet;
        }
        #region Properties
        public Image image;
        StageViewModel ownerStage;
        PhotoflowViewModel photoflowOwner;
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                    photoflowOwner?.NotifyPropertyChanged(nameof(photoflowOwner.IsAnyImageSelected));
                }
            }
        }
        public int Id
        {
            get
            {
                return image.Id;
            }
            set
            {
                if (image.Id != value)
                {
                    image.Id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string FileName
        {
            get
            {
                return image.FileName;
            }
            set
            {
                if (image.FileName != value)
                {
                    image.FileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public byte[] CompressedBytes
        {
            get
            {
                return image.CompressedBytes;
            }
            set
            {
                if (image.CompressedBytes != value)
                {
                    image.CompressedBytes = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion
        #region Commands
        public AsyncCommand ImageClickedCommand { get; set; }
        public AsyncCommand CopyImageCommand { get; set; }
        public AsyncCommand DeleteImageFromStageCommand { get; set; }
        public AsyncCommand DeleteImageCommand { get; set; }
        public AsyncCommand RenameImageCommand { get; set; }
        public AsyncCommand ShowReferencesCommand { get; set; }

        private async Task ImageClicked()
        {
            await dialogService.DisplayMessage("Message");
        }
        private async Task CopyImage()
        {
        }
        private async Task DeleteImageFromStage()
        {
        }
        private async Task DeleteImage()
        {
        }
        private async Task RenameImage()
        {
        }
        private async Task ShowReferencesToPatients()
        {
        }

        #endregion
    }
}
