using System.Threading.Tasks;

namespace ClinicDentClientCommon.Interfaces
{
    public interface IDialogService
    {
        Task DisplayMessage(string message);
        Task<byte[]> PickFile(params string[] formats);
    }
}
