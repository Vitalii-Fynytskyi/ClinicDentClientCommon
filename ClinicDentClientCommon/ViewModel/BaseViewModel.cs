using ClinicDentClientCommon.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClinicDentClientCommon.ViewModel
{
    [Flags]
    public enum ViewModelStatus : byte
    {
        NotChanged = 0,
        Updated = 1,
        Inserted = 2,
        Deleted = 4
    }
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public virtual ViewModelStatus ViewModelStatus
        {
            get
            {
                return viewModelStatus;
            }
            set
            {
                if (viewModelStatus != value)
                {
                    viewModelStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }
        protected ViewModelStatus viewModelStatus = ViewModelStatus.NotChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public INavigate Navigation { get; set; }
        internal BaseViewModel(INavigate navigation) => Navigation = navigation;
    }
}
