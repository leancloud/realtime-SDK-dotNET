using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LeanCloud.Realtime.Test.Integration.WPFNetFx45.Model;
using LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel.ConversationSessionViewModel;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public AVRealtime realtime { get; internal set; }
        public AVIMClient CurrentClient { get; internal set; }


        private UserControl _leftContent;
        private UserControl _centerContent;
        private UserControl _bottomContent;
        private UserControl _rightContent;
        private UserControl _logContent;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Websockets.Net.WebsocketConnection.Link();
            //var config = new AVRealtime.Configuration()
            //{
            //    ApplicationId = "021h1hbtd5shlz38pegnpkmq9d3qf8os1vt0nef4f2lxjru8",
            //    ApplicationKey = "3suek8gdtfk3j3dgb42p9o8jhfjkbnmtefk3z9500balmf2e",
            //    SignatureFactory = new LeanEngineSignatureFactory()
            //};
            string appId = "021h1hbtd5shlz38pegnpkmq9d3qf8os1vt0nef4f2lxjru8";
            string appKey = "3suek8gdtfk3j3dgb42p9o8jhfjkbnmtefk3z9500balmf2e";
            var config = new AVRealtime.Configuration()
            {
                ApplicationId = appId,
                ApplicationKey = appKey,
                //SignatureFactory = new LeanEngineSignatureFactory()
            };
            realtime = new AVRealtime(config);
            realtime.RegisterMessageType<Emoji>();
            realtime.RegisterMessageType<EmojiV2>();
            realtime.RegisterMessageType<BinaryMessage>();

            this.CenterContent = new LogIn();
            var logInVM = ServiceLocator.Current.GetInstance<LogInViewModel>();
            logInVM.realtime = realtime;
            logInVM.client = CurrentClient;

            logInVM.PropertyChanged += LogInVM_PropertyChanged;

            this.LogContent = new WebSocketLog();
            var logVM = ServiceLocator.Current.GetInstance<WebSocketLogViewModel>();

            var teamVM = ServiceLocator.Current.GetInstance<TeamViewModel>();
            teamVM.PropertyChanged += TeamVM_PropertyChanged;
        }

        private async void TeamVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DoSelected")
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.CenterContent = new Chat();
                    this.LeftContent = new ConversationGroup();
                    this.BottomContent = new Compose();
                    this.RightContent = new UserList();
                });

                var userSelectBox = new UserSelectBox();
                var teamVM = ServiceLocator.Current.GetInstance<TeamViewModel>();
                userSelectBox.DataContext = new UserSelectViewModel(teamVM.UsersInTeam);
                var chatVM = ServiceLocator.Current.GetInstance<ChatViewModel>();
                chatVM.UserSelectBox = userSelectBox;
                await chatVM.InitSessionGroups();
            }
        }

        private async void LogInVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected")
            {
                var logInVM = ServiceLocator.Current.GetInstance<LogInViewModel>();
                if (realtime.State == AVRealtime.Status.Online)
                {
                    var chatVM = ServiceLocator.Current.GetInstance<ChatViewModel>();
                    chatVM.client = logInVM.client;
                    chatVM.client.OnSessionClosed += Client_OnSessionClosed;
                }
            }
        }

        private void Client_OnSessionClosed(object sender, AVIMSessionClosedEventArgs e)
        {
            if (e.Code == 4111)
            {
                var logInVM = ServiceLocator.Current.GetInstance<LogInViewModel>();
                logInVM.Reset();
                this.CurrentClient = null;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.CenterContent = new LogIn();
                    this.LeftContent = null;
                    this.BottomContent = null;
                });

                MessageBox.Show("您的 client Id 在别处登录，当前登录失效，连接已断开", "单点登录冲突");
            }
        }

        public UserControl CenterContent
        {
            get { return _centerContent; }
            set
            {
                _centerContent = value;
                RaisePropertyChanged("CenterContent");
            }
        }

        public UserControl LeftContent
        {
            get { return _leftContent; }
            set
            {
                this._leftContent = value;
                RaisePropertyChanged("LeftContent");
            }
        }

        public UserControl BottomContent
        {
            get { return _bottomContent; }
            set
            {
                this._bottomContent = value;
                RaisePropertyChanged("BottomContent");
            }
        }

        public UserControl RightContent
        {
            get { return _rightContent; }
            set
            {
                this._rightContent = value;
                RaisePropertyChanged("RightContent");
            }
        }
        public UserControl LogContent
        {
            get { return _logContent; }
            set
            {
                this._logContent = value;
                RaisePropertyChanged("LogContent");
            }
        }
    }
}