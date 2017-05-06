using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LeanCloud.Realtime.Test.Integration.WPFNetFx45.Controller;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    public class LogInViewModel : ViewModelBase
    {
        public LogInViewModel()
        {
            ConnectAsync = new RelayCommand<object>((passwordBox) => ConnectExecuteAsync(passwordBox));
            GoAsync = new RelayCommand(() => GoExecute(), () => true);
        }
        public AVRealtime realtime { get; internal set; }
        public AVIMClient client { get; internal set; }
        public ICommand ConnectAsync { get; private set; }
        public ICommand GoAsync { get; private set; }

        private async void ConnectExecuteAsync(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            Connecting = true;
            var password = passwordBox.Password;

            var user = await AVUser.LogInAsync(this.ClienId, password);
            this.ButtonText = "欢迎回来, " + user.Username;
            var teamVM = ServiceLocator.Current.GetInstance<TeamViewModel>();
            await teamVM.InitByUserAsync(user);

            client = await realtime.CreateClient(ClienId, tag: Tag, deviceId: DeviceId);
            client.OnOfflineMessageReceived += Client_OnOfflineMessageReceived;
            Connecting = false;
            Connected = true;
        }

        private void Client_OnOfflineMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            Console.WriteLine(e.Message.Id);
        }

        public void Reset()
        {
            Connected = false;
            Connecting = false;
        }
        private void GoExecute()
        {

        }

        private string _clientId;
        public string ClienId
        {
            get
            {
                return _clientId;
            }
            set
            {
                if (_clientId == value)
                    return;
                _clientId = value;
                RaisePropertyChanged("ClienId");
            }
        }

        private string _tag = "pc";
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                if (_tag == value)
                    return;
                _tag = value;
                RaisePropertyChanged("Tag");
            }
        }

        private string _btnText = "登录";
        public string ButtonText
        {
            get
            {
                return _btnText;
            }
            set
            {
                if (_btnText == value)
                    return;
                _btnText = value;
                RaisePropertyChanged("ButtonText");
            }
        }

        private string _deviceId = Guid.NewGuid().ToString();
        public string DeviceId
        {
            get
            {
                return _deviceId;
            }
            set
            {
                if (_deviceId == value)
                    return;
                _deviceId = value;
                RaisePropertyChanged("DeviceId");
            }
        }

        private bool _connecting;
        public bool Connecting
        {
            get
            {
                return _connecting;
            }
            set
            {
                if (_connecting == value)
                    return;
                _connecting = value;
                RaisePropertyChanged("Connecting");
            }
        }

        private bool _connected;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                if (_connected == value)
                    return;
                _connected = value;
                RaisePropertyChanged("Connected");
            }
        }
    }
}
