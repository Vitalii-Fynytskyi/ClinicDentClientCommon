using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using ClinicDentClientCommon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class StageViewModel:BaseViewModel, IDataErrorInfo
    {
        #region IDataErrorInfo_Implementation
        protected string error = string.Empty;
        public string Error => error;
        public string this[string propertyName]
        {
            get
            {
                error = string.Empty;
                if (propertyName == nameof(StageDatetime))
                {
                    bool isValid = DateTime.TryParseExact(StageDatetime, SharedData.DateTimePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
                    if (isValid == false)
                    {
                        error = $"Дата має бути в форматі {SharedData.DateTimePattern}. Етап не буде збережено";
                    }
                }
                return error;
            }
        }
        #endregion
        IClipboardService clipboardService;
        IDialogService dialogService;
        IErrorHandler errorHandler;
        IServiceProvider serviceProvider;
        public AsyncCommand DeleteStageCommand { get; set; }
        public AsyncCommand<string> MarkSentViaMessagerCommand { get; set; }
        public AsyncCommand<string> SendStageViaViberCommand { get; set; }
        public AsyncCommand AddImageFromClipboardCommand { get; set; }
        public AsyncCommand AddImageFromDiskCommand { get; set; }
        public AsyncCommand AttachImageCommand { get; set; }
        public AsyncCommand ObservationClickedCommand { get; set; }

        public StageViewModel(IClipboardService clipboardServiceToSet, IErrorHandler errorHandlerToSet, IDialogService dialogServiceToSet, IServiceProvider serviceProviderToSet, INavigate navigate) : base(navigate)
        {
            serviceProvider = serviceProviderToSet;
            errorHandler = errorHandlerToSet;
            clipboardService = clipboardServiceToSet;
            dialogService = dialogServiceToSet;


            
            DeleteStageCommand = new AsyncCommand(DeleteStage,null,errorHandlerToSet);
            SendStageViaViberCommand = new AsyncCommand<string>(SendStageViaTelegram, null, errorHandlerToSet);
            AddImageFromDiskCommand = new AsyncCommand(AddImageFromDisk, null, errorHandlerToSet);
            AttachImageCommand = new AsyncCommand(AttachImage, null, errorHandlerToSet);
            ObservationClickedCommand = new AsyncCommand(ObservationClicked, null, errorHandlerToSet);

            AddImageFromClipboardCommand = new AsyncCommand(AddImageFromClipboard, null, errorHandlerToSet);
            MarkSentViaMessagerCommand = new AsyncCommand<string>(MarkSentViaMessager, null, errorHandlerToSet);
        }
        public async Task Initialize(Stage stageToSet, StagesViewModel ownerToSet)
        {
            owner = ownerToSet;
            stage = stageToSet;
            stage.OldPrice = stage.Price;
            stage.OldExpenses = stage.Expenses;
            stage.OldPayed = stage.Payed;
            if (ownerToSet.MarkedDate != null)
            {
                if (ownerToSet.MarkedDate == DateTime.ParseExact(stageToSet.StageDatetime, SharedData.DateTimePattern, null).Date)
                {
                    BoundBackground = "PaleGreen";
                }
                else
                {
                    BoundBackground = "Transparent";

                }
            }
            else
            {
                BoundBackground = "Transparent";

            }
            await LoadImages();
        }
        public static async Task LoadAssets()
        {
            List<StageAsset> allAssets = await HttpService.GetStageAssets().ConfigureAwait(false);

            List<StageAsset> operations = allAssets.Where(a => a.Type == AssetType.Operation).ToList();

            List<string> desiredOperationsOrder = new List<string>
            {
                "Реставрація",
                "Пломбування каналів",
                "Цементування коронок",
                "Відновлення культі",
                "Гігієна",
                "Інше"
            };
            // Create a dictionary to map the desired order to indices
            Dictionary<string, int> orderDict = desiredOperationsOrder
                .Select((value, index) => new { value, index })
                .ToDictionary(x => x.value, x => x.index);

            // Sort the operations list based on the desired order
            operations.Sort((x, y) =>
            {
                int indexX = orderDict.ContainsKey(x.Value) ? orderDict[x.Value] : int.MaxValue;
                int indexY = orderDict.ContainsKey(y.Value) ? orderDict[y.Value] : int.MaxValue;
                return indexX.CompareTo(indexY);
            });

            allAssets.RemoveAll(a => a.Type == AssetType.Operation);
            allAssets = allAssets.OrderBy(a => a.Value).ToList();

            StageAsset.Operations = operations;
            StageAsset.Bonds = new List<StageAsset>();
            StageAsset.Enamels = new List<StageAsset>();
            StageAsset.Dentins = new List<StageAsset>();
            StageAsset.CanalMethods = new List<StageAsset>();
            StageAsset.Sealers = new List<StageAsset>();
            StageAsset.Cements = new List<StageAsset>();
            StageAsset.Technicians = new List<StageAsset>();
            StageAsset.Pins = new List<StageAsset>();
            StageAsset.Calciums = new List<StageAsset>();
            foreach (StageAsset asset in allAssets)
            {
                switch (asset.Type)
                {
                    case AssetType.Bond:
                        StageAsset.Bonds.Add(asset);
                        break;
                    case AssetType.Dentin:
                        StageAsset.Dentins.Add(asset);
                        break;
                    case AssetType.Enamel:
                        StageAsset.Enamels.Add(asset);
                        break;
                    case AssetType.CanalMethod:
                        StageAsset.CanalMethods.Add(asset);
                        break;
                    case AssetType.Sealer:
                        StageAsset.Sealers.Add(asset);
                        break;
                    case AssetType.Cement:
                        StageAsset.Cements.Add(asset);
                        break;
                    case AssetType.Technician:
                        StageAsset.Technicians.Add(asset);
                        break;
                    case AssetType.Pin:
                        StageAsset.Pins.Add(asset);
                        break;
                    case AssetType.Calcium:
                        StageAsset.Calciums.Add(asset);
                        break;
                }
            }
        }
        public string Title
        {
            get
            {
                return stage.Title;
            }
            set
            {
                if (stage.Title != value)
                {
                    stage.Title = value;
                    ViewModelStatus = ViewModelStatus.Updated;
                    NotifyPropertyChanged();
                }
            }
        }
        public string StageDatetime
        {
            get
            {
                return stage.StageDatetime;
            }
            set
            {
                if (stage.StageDatetime != value)
                {
                    stage.StageDatetime = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public bool IsSentViaViber
        {
            get
            {
                return stage.IsSentViaViber;
            }
            set
            {
                if (stage.IsSentViaViber != value)
                {
                    stage.IsSentViaViber = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Operation
        {
            get
            {
                return stage.Operation;
            }
            set
            {
                if (stage.Operation != value)
                {
                    stage.Operation = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }
            }
        }
        public StageAsset Cement
        {
            get
            {
                return stage.Cement;
            }
            set
            {
                if (stage.Cement != value)
                {
                    stage.Cement = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Calcium
        {
            get
            {
                return stage.Calcium;
            }
            set
            {
                if (stage.Calcium != value)
                {
                    stage.Calcium = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Bond
        {
            get
            {
                return stage.Bond;
            }
            set
            {
                if (stage.Bond != value)
                {
                    stage.Bond = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Pin
        {
            get
            {
                return stage.Pin;
            }
            set
            {
                if (stage.Pin != value)
                {
                    stage.Pin = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Dentin
        {
            get
            {
                return stage.Dentin;
            }
            set
            {
                if (stage.Dentin != value)
                {
                    stage.Dentin = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Enamel
        {
            get
            {
                return stage.Enamel;
            }
            set
            {
                if (stage.Enamel != value)
                {
                    stage.Enamel = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset CanalMethod
        {
            get
            {
                return stage.CanalMethod;
            }
            set
            {
                if (stage.CanalMethod != value)
                {
                    stage.CanalMethod = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }
            }
        }
        public StageAsset Sealer
        {
            get
            {
                return stage.Sealer;
            }
            set
            {
                if (stage.Sealer != value)
                {
                    stage.Sealer = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public StageAsset Technician
        {
            get
            {
                return stage.Technician;
            }
            set
            {
                if (stage.Technician != value)
                {
                    stage.Technician = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public string DoctorName
        {
            get
            {
                return stage.DoctorName;
            }
            set
            {
                if (stage.DoctorName != value)
                {
                    stage.DoctorName = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public string CommentText
        {
            get
            {
                return stage.CommentText;
            }
            set
            {
                if (stage.CommentText != value)
                {
                    stage.CommentText = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }

            }
        }
        public int Payed
        {
            get
            {
                return stage.Payed;
            }
            set
            {
                if (stage.Payed != value)
                {
                    stage.Payed = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    UpdateImagePaymentStatus();
                    NotifyPropertyChanged();
                }
            }
        }
        public int? ToothUnderObservationId
        {
            get
            {
                return stage.ToothUnderObservationId;
            }
            set
            {
                if (value != stage.ToothUnderObservationId)
                {
                    stage.ToothUnderObservationId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Expenses
        {
            get
            {
                return stage.Expenses;
            }
            set
            {
                if (stage.Expenses != value)
                {
                    stage.Expenses = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    NotifyPropertyChanged();
                }
            }
        }
        public Stage Stage
        {
            set
            {
                if (stage != value)
                {
                    stage = value;
                    NotifyPropertyChanged(nameof(Stage), nameof(Title), nameof(CommentText), nameof(StageDatetime), nameof(IsSentViaViber), nameof(Operation), nameof(Cement), nameof(Calcium), nameof(Bond), nameof(Dentin), nameof(Pin), nameof(Enamel), nameof(CanalMethod), nameof(Sealer), nameof(Technician), nameof(Price), nameof(Payed), nameof(Expenses), nameof(ToothUnderObservationId), nameof(DoctorName));
                }
            }
            get
            {
                return stage;
            }
        }
        private Stage stage;
        public int Price
        {
            get
            {
                return stage.Price;
            }
            set
            {
                if (stage.Price != value)
                {
                    stage.Price = value;
                    ViewModelStatus = ViewModelStatus.Updated;

                    UpdateImagePaymentStatus();
                    NotifyPropertyChanged();
                }

            }
        }
        public bool IsOwner
        {
            get
            {
                return DoctorId == SharedData.CurrentDoctor.Id;
            }
        }
        public int DoctorId
        {
            get
            {
                return stage.DoctorId;
            }
        }
        public string BoundBackground
        {
            get
            {
                return boundBackground;
            }
            set
            {
                if (boundBackground != value)
                {
                    boundBackground = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string boundBackground;

        private ObservableCollection<ImageViewModel> images;
        public ObservableCollection<ImageViewModel> Images
        {
            get
            {
                return images;
            }
            set
            {
                if (images != value)
                {
                    images = value;
                    NotifyPropertyChanged();
                }

            }
        }
        private void UpdateImagePaymentStatus()
        {
            if (paymentStatusImagePath == "ok.png" && Price != Payed)
            {
                NotifyPropertyChanged("PaymentStatusImagePath");
            }
            else if (paymentStatusImagePath == "wrong.png" && Price == Payed)
            {
                NotifyPropertyChanged("PaymentStatusImagePath");
            }
        }
        private string paymentStatusImagePath = "ok.png";
        public string PaymentStatusImagePath
        {
            get
            {
                if (Price != Payed)
                {
                    paymentStatusImagePath = "wrong.png";
                    return paymentStatusImagePath;
                }
                else
                {
                    paymentStatusImagePath = "ok.png";
                    return paymentStatusImagePath;
                }
            }
        }

        private async Task LoadImages()
        {
            Images = new ObservableCollection<ImageViewModel>();
            Image[] images = await HttpService.GetImagesForStage(stage.Id);
            foreach (Image image in images)
            {
                ImageViewModel imageViewModel = (ImageViewModel)serviceProvider.GetService(typeof(ImageViewModel));
                await imageViewModel.Initialize(image,this);
                Images.Add(imageViewModel);
            }
        }
        private async Task MarkSentViaMessager(string obj)
        {
            int mark = Convert.ToInt32(obj);
            HttpService.StageMarkSentViaMessager(stage.Id, mark);
            IsSentViaViber = Convert.ToBoolean(mark);
        }
        private StagesViewModel owner;

        private async Task AddImageFromClipboard()
        {
            
        }
        private async Task DeleteStage()
        {

        }
        private async Task SendStageViaTelegram(string phone)
        {

            
        }
        private async Task AddImageFromDisk()
        {
            
        }
        private async Task AttachImage()
        {
            
        }
        private async Task ObservationClicked()
        {

        }

        public async Task AttachImages(IEnumerable<ImageViewModel> images)
        {
            
        }
    }
}
