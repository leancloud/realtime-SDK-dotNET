using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LeanCloud.Core.Internal;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        public ICommand CreateConvesationAsync { get; private set; }
        public ICommand ShowAllMembers { get; private set; }
        public ChatViewModel()
        {

            this.SessionGroups = new ObservableCollection<ConversationGroupViewModel>();

            SessionGroups.Add(new ConversationGroupViewModel() { Name = "群聊", Category = 1, Sessions = new ObservableCollection<ConversationSessionViewModel>() });
            SessionGroups.Add(new ConversationGroupViewModel() { Name = "私聊", Category = 0, Sessions = new ObservableCollection<ConversationSessionViewModel>() });
            SessionGroups.Add(new ConversationGroupViewModel() { Name = "未加入", Category = -1, Sessions = new ObservableCollection<ConversationSessionViewModel>() });

            this.SelectedSession = new ConversationSessionViewModel()
            {
                Name = "所有人",
            };
        }

        private ConversationSessionViewModel _selectedSession;
        public ConversationSessionViewModel SelectedSession
        {
            get
            {
                return _selectedSession;
            }
            set
            {
                _selectedSession = value;
                RaisePropertyChanged("SelectedSession");
            }
        }

        private ObservableCollection<ConversationGroupViewModel> _sessionGroups;
        public ObservableCollection<ConversationGroupViewModel> SessionGroups
        {
            get
            {
                return _sessionGroups;
            }
            set
            {
                _sessionGroups = value;
                RaisePropertyChanged("SessionGroups");
            }
        }

        public AVIMClient client { get; internal set; }
        private UserControl _userSelectBox;
        public UserControl UserSelectBox
        {
            get { return _userSelectBox; }
            set
            {
                this._userSelectBox = value;
                RaisePropertyChanged("UserSelectBox");
            }
        }

        public async Task InitSessionGroups()
        {
            var teamVM = ServiceLocator.Current.GetInstance<TeamViewModel>();
            var conversationQuery = new AVQuery<AVObject>("_Conversation")
                .WhereEqualTo("team", teamVM.SelectdTeam).Limit(200);

            var conversations = (await conversationQuery.FindAsync())
                .Select(x => AVIMConversation.CreateWithData(x, client));

            foreach (var conversation in conversations)
            {
                var session = new ConversationSessionViewModel(conversation);
                if (conversation.ContainsKey("category"))
                {
                    var category = conversation.Get<int>("category");
                    if (conversation.MemberIds.Contains(client.ClientId))
                    {
                        var group = this.SessionGroups.First(g => g.Category == category);
                        if (group != null)
                        {
                            group.Sessions.Add(session);
                        }
                    }
                    else
                    {
                        var group = this.SessionGroups.First(g => g.Category == -1);
                        if (group != null)
                        {
                            group.Sessions.Add(session);
                        }
                    }
                }
            }
        }
        public void CategoryClassify(ConversationSessionViewModel conversationVM)
        {
            var newHere = true;
            foreach (var sg in SessionGroups)
            {
                foreach (var session in sg.Sessions)
                {
                    if (session.Equals(conversationVM))
                    {
                        if (conversationVM.Category == session.Category)
                        {
                            return;
                        }
                        else
                        {
                            newHere = false;
                        }
                    }
                }
            }
            if (conversationVM.ConversationInSession.ContainsKey("category"))
            {
                var category = conversationVM.ConversationInSession.Get<int>("category");
                if (conversationVM.ConversationInSession.MemberIds.Contains(client.ClientId))
                {
                    var group = this.SessionGroups.First(g => g.Category == category);
                    if (group != null)
                    {
                        group.Sessions.Add(conversationVM);
                    }
                }
                else
                {
                    var group = this.SessionGroups.First(g => g.Category == -1);
                    if (group != null)
                    {
                        group.Sessions.Add(conversationVM);
                    }
                }
            }
        }

        public async Task CreateConversationExecuteAsync(UserSelectViewModel selected)
        {
            var members = selected.SelectedUsers
                .Where(x => x.IsSelected)
                .Select(u => u.UserInfo.Name)
                .Concat(new string[] { client.ClientId });

            // 群聊 ：0 
            // 私聊：1
            var category = members.Count() > 3 ? 0 : 1;
            var options = new Dictionary<string, object>();
            var name = string.Join(",", members) + " 的群聊";
            if (category == 1)
            {
                name = string.Join("和", members) + " 的私聊";
            }
            var teamVM = ServiceLocator.Current.GetInstance<TeamViewModel>();
            options.Add("team", teamVM.SelectdTeam);
            options.Add("category", category);
            var newConversation = await client.CreateConversationAsync(members: members, name: name, options: options);
            var newSession = new ConversationSessionViewModel()
            {
                ConversationInSession = newConversation,
            };
            await newSession.LoadUsersInConversationAsync();
            var group = this.SessionGroups.First(x => x.Category == 0);
            group.Sessions.Add(newSession);
        }
        public async Task OpenConversationExecuteAsync(ConversationSessionViewModel selected)
        {
            if (this.SelectedSession.Equals(selected)) return;
            this.SelectedSession = selected;

            await this.SelectedSession.LoadHistoryAsync(init: true, limit: 20);
            await this.SelectedSession.LoadUsersInConversationAsync(init: true);
        }
    }

    /// <summary>
    /// 对话分组，类似于用户标签，比如我的家人，我的朋友这种分组
    /// </summary>
    public class ConversationGroupViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        private int _category;
        public int Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (_category == value)
                    return;
                _category = value;
                RaisePropertyChanged("Category");
            }
        }
        private ObservableCollection<ConversationSessionViewModel> _sessions;
        public ObservableCollection<ConversationSessionViewModel> Sessions
        {
            get
            {
                return _sessions;
            }
            set
            {
                if (_sessions == value)
                    return;
                _sessions = value;
                RaisePropertyChanged("Sessions");
            }
        }
    }

    public class ConversationSessionViewModel : ViewModelBase
    {
        public ConversationSessionViewModel(AVIMConversation conversation)
            : this()
        {
            this.ConversationInSession = conversation;
            Name = this.ConversationInSession.Name;

            conversation.CurrentClient.OnMessageReceived += CurrentClient_OnMessageReceived;

            conversation.CurrentClient.OnMembersJoined += CurrentClient_OnMembersJoined;

            conversation.CurrentClient.OnInvited += CurrentClient_OnInvited;



            MessageQueue = new SnackbarMessageQueue();
        }

        private void CurrentClient_OnInvited(object sender, AVIMOnInvitedEventArgs e)
        {
            if (e.ConversationId == this.ConversationInSession.ConversationId)
            {
                var messageFormatTeamplate = "你被 {0} 邀请了加入到对话";
                var messageContent = string.Format(messageFormatTeamplate, e.InvitedBy);
                this.MessageQueue.Enqueue(messageContent);
            }
        }

        private void CurrentClient_OnMembersJoined(object sender, AVIMOnMembersJoinedEventArgs e)
        {
            if (e.ConversationId == this.ConversationInSession.ConversationId)
            {
                var messageFormatTeamplate = "{0} 邀请了{1} 加入到对话";
                var memberListString = string.Join(",", e.JoinedMembers);
                var messageContent = string.Format(messageFormatTeamplate, e.InvitedBy, memberListString);
                this.MessageQueue.Enqueue(messageContent);
            }
        }

        private void CurrentClient_OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if (e.Message.ConversationId == this.ConversationInSession.ConversationId)
            {
                if (e.Message is AVIMTextMessage)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var item = new MessageViewModel(iMessage: e.Message);
                        MessagesInSession.Add(item);
                        this.SelectedItem = item;
                    });
                }
                else if (e.Message is BinaryMessage)
                {
                    var binaryMessage = e.Message as BinaryMessage;
                    var binaryData = binaryMessage.BinaryData;
                    var text = System.Text.Encoding.UTF8.GetString(binaryData);
                }
                else
                {

                }
            }
        }

        private void TextMessageListener_OnTextMessageReceived(object sender, AVIMTextMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                var item = new MessageViewModel(e.TextMessage);
                MessagesInSession.Add(item);
                this.SelectedItem = item;
            });
        }
        public SnackbarMessageQueue MessageQueue
        {
            get;
            set;
        }
        private bool _userInited;
        private bool _messageInited;
        public ICommand SendAsync { get; private set; }
        public ICommand RunInviteDialogCommand { get; private set; }
        public ICommand InviteAsync { get; private set; }
        public ICommand OnClicked { get; private set; }
        public ICommand OnStartEditName { get; private set; }
        public ICommand SaveAsync { get; private set; }
        public ICommand OnCancelEditName { get; private set; }
        public ICommand Quit { get; private set; }

        public ConversationSessionViewModel()
        {
            SendAsync = new RelayCommand(() => SendExecuteAsync(), () => true);
            RunInviteDialogCommand = new RelayCommand(() => ExecuteRunDialog(), () => true);
            InviteAsync = new RelayCommand(() => InviteExecuteAsync(), () => true);
            OnClicked = new RelayCommand(() => OnClickExecuteAsync(), () => true);
            OnStartEditName = new RelayCommand(() => StartEditName(), () => true);
            SaveAsync = new RelayCommand(() => SaveConversationAsync(), () => true);
            OnCancelEditName = new RelayCommand(() => CancelEditName(), () => true);
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private bool _nameEditing;
        public bool NameEditing
        {
            get
            {
                return _nameEditing;
            }
            set
            {
                if (_nameEditing == value)
                    return;
                _nameEditing = value;
                RaisePropertyChanged("NameEditing");
                RaisePropertyChanged("Display");
            }
        }
        public bool Display
        {
            get
            {
                return !_nameEditing;
            }
        }

        private string _inputText;
        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                if (_inputText == value)
                    return;
                _inputText = value;
                RaisePropertyChanged("InputText");
            }
        }



        private async void SendExecuteAsync()
        {
            var textMessage = new AVIMTextMessage(this.InputText);
            //await ConversationInSession.SendMessageAsync(textMessage);

            var emojiMessage = new Emoji()
            {
                Ecode = "#e001",
            };
            //await ConversationInSession.SendMessageAsync(emojiMessage);

            var emojiV2Message = new EmojiV2("#e001");
            //await ConversationInSession.SendMessageAsync(emojiV2Message);

            var text = "I love Unity";
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            var binaryMessage = new BinaryMessage(textBytes);
            await ConversationInSession.SendMessageAsync(binaryMessage);
            //await SendBinaryMessageAsync();
            //App.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    var item = new MessageViewModel(textMessage);
            //    MessagesInSession.Add(item);
            //    this.SelectedItem = item;
            //});
            this.InputText = "";
        }
        /// <summary>
        /// 二进制消息
        /// </summary>
        [AVIMMessageClassName("BinaryMessage")]
        public class BinaryMessage : IAVIMMessage
        {
            public BinaryMessage()
            {

            }
            /// <summary>
            /// 从 bytes[] 构建一条消息
            /// </summary>
            /// <param name="data"></param>
            public BinaryMessage(byte[] data)
            {
                BinaryData = data;
            }

            public byte[] BinaryData { get; set; }

            public string ConversationId
            {
                get; set;
            }

            public string FromClientId
            {
                get; set;
            }

            public string Id
            {
                get; set;
            }

            public long RcpTimestamp
            {
                get; set;
            }

            public long ServerTimestamp
            {
                get; set;
            }

            public IAVIMMessage Deserialize(string msgStr)
            {
                var spiltStrs = msgStr.Split(':');
                this.BinaryData = System.Convert.FromBase64String(spiltStrs[1]);
                return this;
            }

            public string Serialize()
            {
                return "bin:" + System.Convert.ToBase64String(this.BinaryData);
            }

            public bool Validate(string msgStr)
            {
                var spiltStrs = msgStr.Split(':');
                return spiltStrs[0] == "bin";
            }
        }
        [AVIMMessageClassName("EmojiV2")]
        public class EmojiV2 : AVIMMessage
        {
            public EmojiV2()
            {

            }
            public EmojiV2(string ecode)
            {
                Content = ecode;
            }
        }

        [AVIMMessageClassName("Emoji")]
        public class Emoji : AVIMTypedMessage
        {
            [AVIMMessageFieldName("Ecode")]
            public string Ecode { get; set; }
        }

        //public class Emoji : IAVIMMessage
        //{
        //    public IDictionary<string, object> Body
        //    {
        //        get; set;
        //    }

        //    public string ConversationId
        //    {
        //        get; set;
        //    }

        //    public string FromClientId
        //    {
        //        get; set;
        //    }

        //    public string Id
        //    {
        //        get; set;
        //    }

        //    public long RcpTimestamp
        //    {
        //        get; set;
        //    }

        //    public long ServerTimestamp
        //    {
        //        get; set;
        //    }

        //    public Task<IDictionary<string, object>> MakeAsync()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IAVIMMessage Restore(IDictionary<string, object> msg)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public bool Validate(IDictionary<string, object> msg)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        private async Task SendJsonBody()
        {
            //IDictionary<string, object> messageBody = new Dictionary<string, object>()
            //{
            //    {"key1","value1" },
            //    {"key2",2 },
            //    {"key3",true },
            //    {"key4",DateTime.Now },
            //    {"key5",new List<string>() { "str1","str2","str3"} },
            //};
            //await ConversationInSession.SendMessageAsync(message);
        }
        //private async Task SendBinaryMessageAsync()
        //{
        //    var text = "I love Unity";
        //    var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        //    var binaryMessage = new BinaryMessage(textBytes);
        //    var afterEncode = binaryMessage.EncodeForSending();
        //    await ConversationInSession.SendMessageAsync(afterEncode);
        //}
        private async void ExecuteRunDialog()
        {
            this.MemberInviteBox = new UserSelectBox()
            {
                DataContext = new UserSelectViewModel(_userInfo: this.UsersInConversation.AsEnumerable())
            };

            var result = await DialogHost.Show(this.MemberInviteBox, "RootDialog", ClosingEventHandler);
        }

        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            var selectedMembers = ((this.MemberInviteBox.DataContext as UserSelectViewModel).SelectedUsers.Where(x => x.IsSelected));
            if (selectedMembers != null)
            {
                if (selectedMembers.Count() > 0)
                {
                    var selectedClientIds = selectedMembers.Select(x => x.UserInfo.Name);
                    await this.ConversationInSession.AddMembersAsync(clientIds: selectedClientIds);
                    selectedMembers.ToList().ForEach(x =>
                    {
                        this.UsersInConversation.Add(x.UserInfo);
                    });
                }
            }
        }
        private async void InviteExecuteAsync()
        {

        }
        private async void QuitAsync()
        {
            await this.ConversationInSession.QuitAsync();
            this.MessageQueue.Enqueue("您已经从 " + this.ConversationInSession.Name + "退出了");

            var chatVM = ServiceLocator.Current.GetInstance<ChatViewModel>();
            chatVM.CategoryClassify(this);
        }
        private UserControl _memberInviteBox;
        public UserControl MemberInviteBox
        {
            get { return _memberInviteBox; }
            set
            {
                this._memberInviteBox = value;
                RaisePropertyChanged("MemberInviteBox");
            }
        }
        private async void OnClickExecuteAsync()
        {
            var chatVM = ServiceLocator.Current.GetInstance<ChatViewModel>();
            await chatVM.OpenConversationExecuteAsync(this);
        }
        private void StartEditName()
        {
            this.NameEditing = true;
        }
        private void CancelEditName()
        {
            this.NameEditing = false;
        }
        private async void SaveConversationAsync()
        {
            this.ConversationInSession.Name = this.Name;
            await this.ConversationInSession.SaveAsync();
            this.NameEditing = false;
        }

        public AVIMConversation ConversationInSession { get; set; }
        public int Category { get; set; }
        private ObservableCollection<UserInfoViewModel> _usersInConversation;
        public ObservableCollection<UserInfoViewModel> UsersInConversation
        {
            get
            {
                return _usersInConversation;
            }
            set
            {
                _usersInConversation = value;
                RaisePropertyChanged("UsersInConversation");
            }
        }

        private ObservableCollection<MessageViewModel> _messagesInSession;

        public ObservableCollection<MessageViewModel> MessagesInSession
        {
            get
            {
                return _messagesInSession;
            }
            set
            {
                _messagesInSession = value;
                RaisePropertyChanged("MessagesInSession");
            }
        }
        private MessageViewModel _selectedItem;
        private MessageViewModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        public async Task LoadHistoryAsync(int limit = 20, bool init = false)
        {
            if (!init && _messageInited) return;
            if (init)
            {
                MessagesInSession = new ObservableCollection<MessageViewModel>();
                if (ConversationInSession == null) return;
                var messages = await ConversationInSession.QueryMessageAsync(limit: limit);
                messages.ToList().ForEach(x =>
                {
                    MessagesInSession.Add(new MessageViewModel(iMessage: x));
                });
                _messageInited = true;
            }
        }
        public async Task LoadUsersInConversationAsync(int limit = 20, bool init = false)
        {
            if (!init && _userInited) return;
            if (init)
            {
                UsersInConversation = new ObservableCollection<UserInfoViewModel>();
                var users = await new AVQuery<AVUser>().Limit(limit).FindAsync();
                var onlineStatus = await ConversationInSession.CurrentClient.PingAsync(targetClientIds: users.Select(x => x.Username));
                users.ToList().ForEach(u =>
                {
                    UsersInConversation.Add(new UserInfoViewModel(u) { IsOnline = onlineStatus.First(x => x.Item1 == u.Username).Item2 });
                });
                _userInited = true;
            }
        }
    }

    public class UserInfoViewModel : ViewModelBase
    {
        private AVUser _user;
        public UserInfoViewModel(AVUser user)
        {
            _user = user;
            this.Name = user.Username;
            this.Abbreviation = this.Name.First().ToString();
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string _abbreviation;
        public string Abbreviation
        {
            get { return _abbreviation; }
            set
            {
                if (_abbreviation == value) return;
                _abbreviation = value;
                RaisePropertyChanged("Abbreviation");
            }
        }
        private bool _isOnline;
        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (_isOnline == value) return;
                _isOnline = value;
                RaisePropertyChanged("IsOnline");
            }
        }
    }

    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel(IAVIMMessage iMessage, string text = null)
        {
            this.Text = "当前客户端不支持显示此类型消息。";
            if (iMessage != null)
            {
                if (iMessage is AVIMTextMessage)
                {
                    this.Text = ((AVIMTextMessage)iMessage).TextContent;
                }
                else if (iMessage is AVIMMessage)
                {
                }

                this.Sender = iMessage.FromClientId;
            }
            if (text != null)
            {
                this.Text = text;
            }
            if (this.Sender != null)
                this.Code = this.Sender.First();
        }

        private bool _served;
        public bool Served
        {

            get
            {
                return _served;
            }
            set
            {
                if (_served == value)
                    return;
                _served = value;
                RaisePropertyChanged("Served");
            }
        }

        public bool _read;
        public bool Read
        {

            get
            {
                return _read;
            }
            set
            {
                if (_read == value)
                    return;
                _read = value;
                RaisePropertyChanged("Read");
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        private string _sender;
        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                if (_sender == value)
                    return;
                _sender = value;
                RaisePropertyChanged("Sender");
            }
        }

        private char _code;
        public char Code
        {
            get { return _code; }
            set
            {
                if (_code == value) return;
                _code = value;
                RaisePropertyChanged("Code");
            }
        }
    }
}
