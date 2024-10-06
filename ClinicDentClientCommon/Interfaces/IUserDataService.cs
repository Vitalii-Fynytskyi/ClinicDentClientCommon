using ClinicDentClientCommon.Model;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.Interfaces
{
    public interface IUserDataService
    {
        Task<LoginModel> ReadLoginModel();
        void WriteLoginModel(LoginModel loginModel);
        void DeleteLoginModel();
        Task<Settings> ReadSettings();
        void WriteSettings(Settings settings);
    }
}
