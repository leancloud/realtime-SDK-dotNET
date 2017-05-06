using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    public class WebSocketLogViewModel : ViewModelBase
    {
        public WebSocketLogViewModel()
        {
            AVRealtime.WebSocketLog(AppendLog);
        }
        private StringBuilder sbLog = new StringBuilder();
        public void AppendLog(string log)
        {
            sbLog.AppendLine(log);
            RaisePropertyChanged("Log");
        }
        public string Log
        {
            get { return sbLog.ToString(); }
            set { sbLog.Clear(); }
        }
    }
}
