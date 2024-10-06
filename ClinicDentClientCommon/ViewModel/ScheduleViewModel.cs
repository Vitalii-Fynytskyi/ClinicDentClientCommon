using ClinicDentClientCommon.Commands;
using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using System;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class ScheduleViewModel : BaseViewModel
    {
        IErrorHandler errorHandler;
        public ScheduleViewModel(IErrorHandler errorHandlerToSet,INavigate navigation) : base(navigation)
        {
            errorHandler=errorHandlerToSet;
            NavigateToScheduleScreenCommand = new AsyncCommand(NavigateToScheduleScreen, null, errorHandlerToSet);
        }
        public async Task Initialize(Schedule scheduleToSet)
        {
            schedule = scheduleToSet;
            startDateTimeDT = DateTime.ParseExact(scheduleToSet.StartDatetime, SharedData.DateTimePattern, null);
        }
        #region Commands
        public AsyncCommand NavigateToScheduleScreenCommand { get; set; }
        public async Task NavigateToScheduleScreen()
        {

        }
        #endregion
        #region Properties
        public Schedule schedule;
        public string StartDateTime
        {
            get
            {
                return schedule.StartDatetime;
            }
            set
            {
                if (value != schedule.StartDatetime)
                {
                    schedule.StartDatetime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DateTime startDateTimeDT;
        public DateTime StartDateTimeDT
        {
            get
            {
                return startDateTimeDT;
            }
            set
            {
                if (value != startDateTimeDT)
                {
                    startDateTimeDT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int Id
        {
            get
            {
                return schedule.Id;
            }
        }
        public int CabinetId
        {
            get
            {
                return schedule.CabinetId;
            }
        }
        public string CabinetName
        {
            get
            {
                return schedule.CabinetName;
            }
        }
        #endregion

    }
}
