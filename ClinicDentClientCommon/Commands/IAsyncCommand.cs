﻿using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicDentClientCommon.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}
