using System.Collections.Generic;
using System.Threading.Tasks;
namespace ClinicDentClientCommon.Interfaces
{
    public interface INavigate
    {
        Task NavigateTo(string route, Dictionary<string, object> parameters=null);
        Task PushModal<T>(T page);
        Task PopModal();
    }
}
