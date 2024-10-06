using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using ClinicDentClientCommon.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicDentClientCommon.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        LoginModel loginModel;
        public IUserDataService userDataService;
        public IDialogService dialogService;
        public IErrorHandler errorHandler;


        public AsyncCommand LoginCommand { get; set; }
        public async Task Login()
        {
            IsWaitingServerResponse = true;
            Doctor doctor = await HttpService.Authenticate(loginModel);
            if (doctor == null)
            {
                dialogService?.DisplayMessage("Неправильно введено логін або пароль");
                IsWaitingServerResponse = false;
                return;
            }
            await StageViewModel.LoadAssets();
            userDataService.WriteLoginModel(loginModel);
            IsWaitingServerResponse = false;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("doctor", doctor);
            await Navigation.NavigateTo("", dictionary);

        }
        public bool CanLogin()
        {
            return IsWaitingServerResponse != true && Email != null && Email.Length >= 3 && Password != null && Password.Length >= 3 && Tenant != null;
        }
        public string Email
        {
            get
            {
                return loginModel.Email;
            }
            set
            {
                if (loginModel.Email != value)
                {
                    loginModel.Email = value;
                    NotifyPropertyChanged();
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string Password
        {
            get
            {
                return loginModel.Password;
            }
            set
            {
                if (loginModel.Password != value)
                {
                    loginModel.Password = value;
                    NotifyPropertyChanged();
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string Tenant
        {
            get
            {
                return loginModel.Tenant;
            }
            set
            {
                if (loginModel.Tenant != value)
                {
                    loginModel.Tenant = value;
                    NotifyPropertyChanged();
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string[] Tenants
        {
            get
            {
                return tenants;
            }
            set //updates tenant list in UI, try to select default tenant from settings
            {

                if (tenants != value)
                {
                    tenants = value;
                    NotifyPropertyChanged();
                    loginModel.Tenant = null;
                    if (tenants != null && tenants.Length > 0)
                    {
                        string defaultTenant = SharedData.Settings.DefaultTenant;
                        if (tenants.Contains(defaultTenant))
                        {
                            loginModel.Tenant = defaultTenant;
                        }
                        else
                        {
                            loginModel.Tenant = tenants[0];
                        }
                    }
                    NotifyPropertyChanged("Tenant");
                }
            }
        }
        public bool IsWaitingServerResponse
        {
            get
            {
                return isWaitingServerResponse;
            }
            set
            {
                if(value != isWaitingServerResponse)
                {
                    isWaitingServerResponse = value;
                    NotifyPropertyChanged();
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool isWaitingServerResponse = false;
        private string[] tenants;

        public LoginViewModel(IUserDataService userDataServiceToSet, IDialogService dialogServiceToSet, IErrorHandler errorHandlerToSet,INavigate navigation) : base(navigation)
        {
            userDataService = userDataServiceToSet;
            dialogService = dialogServiceToSet;
            errorHandler = errorHandlerToSet;
            LoginCommand = new AsyncCommand(Login, CanLogin, errorHandlerToSet);
            loginModel = new LoginModel();
            GetTenantList();

        }
        public async Task GetTenantList()
        {
            Tenants = await HttpService.GetTenantList();
        }
    }
}
