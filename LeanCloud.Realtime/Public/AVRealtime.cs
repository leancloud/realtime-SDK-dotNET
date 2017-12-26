using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LeanCloud;
using System.Reflection;
using LeanCloud.Realtime.Internal;
using LeanCloud.Storage.Internal;
using System.Threading;
using LeanCloud.Core.Internal;

namespace LeanCloud.Realtime
{

    /// <summary>
    /// 实时消息的框架类
    /// 包含了 WebSocket 连接以及事件通知的管理
    /// </summary>
    public class AVRealtime
    {
        private static readonly object mutex = new object();
        private string _wss;
        private string _sesstionToken;
        private long _sesstionTokenExpire;
        private string _clientId;
        private string _deviceId;
        private bool _secure;
        private string _tag;
        private string subprotocolPrefix = "lc.json.";

        public bool IsSesstionTokenExpired
        {
            get
            {
                return DateTime.Now.UnixTimeStampSeconds() > _sesstionTokenExpire;
            }
        }



        private IAVIMCommandRunner avIMCommandRunner;


        /// <summary>
        /// 
        /// </summary>
        public IAVIMCommandRunner AVIMCommandRunner
        {
            get
            {
                lock (mutex)
                {
                    avIMCommandRunner = avIMCommandRunner ?? new AVIMCommandRunner(this.PCLWebsocketClient);
                    return avIMCommandRunner;
                }
            }
        }

        private IWebSocketClient webSocketController;
        internal IWebSocketClient PCLWebsocketClient
        {
            get
            {
                lock (mutex)
                {
                    webSocketController = webSocketController ?? new DefaultWebSocketClient();
                    return webSocketController;

                }
            }
            set
            {
                lock (mutex)
                {
                    webSocketController = value;
                }
            }
        }

        internal static IAVRouterController RouterController
        {
            get
            {
                return AVIMCorePlugins.Instance.RouterController;
            }
        }

        internal static IFreeStyleMessageClassingController FreeStyleMessageClassingController
        {
            get
            {
                return AVIMCorePlugins.Instance.FreeStyleClassingController;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AVIMMessageEventArgs> OnOfflineMessageReceived;

        /// <summary>
        /// 与云端通讯的状态
        /// </summary>
        public enum Status : int
        {
            /// <summary>
            /// 未初始化
            /// </summary>
            None = -1,

            /// <summary>
            /// 正在连接
            /// </summary>
            Connecting = 0,

            /// <summary>
            /// 已连接
            /// </summary>
            Online = 1,

            /// <summary>
            /// 连接已断开
            /// </summary>
            Offline = 2,

            /// <summary>
            /// 正在重连
            /// </summary>
            Reconnecting = 3,

            /// <summary>
            /// websocket 连接已被打开
            /// </summary>
            Opened = 98,

            /// <summary>
            /// 已主动关闭
            /// </summary>
            Closed = 99,
        }

        private AVRealtime.Status state = Status.None;
        public AVRealtime.Status State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct AVIMReconnectOptions
        {
            /// <summary>
            /// 重连的时间间隔，单位是秒
            /// </summary>
            public long Interval { get; set; }

            /// <summary>
            /// 重连的次数
            /// </summary>
            public int Retry { get; set; }
        }

        internal string Subprotocol
        {
            get
            {

                return subprotocolPrefix + (int)CurrentConfiguration.OfflineMessageStrategy;
            }
        }

        /// <summary>
        /// 重连选项
        /// </summary>
        public AVIMReconnectOptions ReconnectOptions { get; set; }

        private ISignatureFactory _signatureFactory;

        /// <summary>
        /// 签名接口
        /// </summary>
        public ISignatureFactory SignatureFactory
        {
            get
            {
                _signatureFactory = _signatureFactory ?? new DefaulSiganatureFactory();
                return _signatureFactory;
            }
            set
            {
                _signatureFactory = value;
            }
        }

        private bool useLeanEngineSignaturFactory;
        /// <summary>
        /// 启用 LeanEngine 云函数签名
        /// </summary>
        public void UseLeanEngineSignatureFactory()
        {
            useLeanEngineSignaturFactory = true;
            this.SignatureFactory = new LeanEngineSignatureFactory();
        }

        private EventHandler<AVIMDisconnectEventArgs> m_OnDisconnected;
        /// <summary>
        /// 连接断开触发的事件
        /// <remarks>如果其他客户端使用了相同的 Tag 登录，就会导致当前用户被服务端断开</remarks>
        /// </summary>
        public event EventHandler<AVIMDisconnectEventArgs> OnDisconnected
        {
            add
            {
                m_OnDisconnected += value;
            }
            remove
            {
                m_OnDisconnected -= value;
            }
        }

        private EventHandler<AVIMReconnectingEventArgs> m_OnReconnecting;
        /// <summary>
        /// 正在重连时触发的事件
        /// </summary>
        public event EventHandler<AVIMReconnectingEventArgs> OnReconnecting
        {
            add
            {
                m_OnReconnecting += value;
            }
            remove
            {
                m_OnReconnecting -= value;
            }
        }

        private EventHandler<AVIMReconnectedEventArgs> m_OnReconnected;
        /// <summary>
        /// 重连之后触发的事件
        /// </summary>
        public event EventHandler<AVIMReconnectedEventArgs> OnReconnected
        {
            add
            {
                m_OnReconnected += value;
            }
            remove
            {
                m_OnReconnected -= value;
            }
        }

        private EventHandler<AVIMReconnectFailedArgs> m_OnReconnectFailed;

        /// <summary>
        /// 重连失败之后触发的事件
        /// </summary>
        public event EventHandler<AVIMReconnectFailedArgs> OnReconnectFailed
        {
            add
            {
                m_OnReconnectFailed += value;
            }
            remove
            {
                m_OnReconnectFailed -= value;
            }
        }

        private EventHandler<AVIMNotice> m_NoticeReceived;
        public event EventHandler<AVIMNotice> NoticeReceived
        {
            add
            {
                m_NoticeReceived += value;
            }
            remove
            {
                m_NoticeReceived -= value;
            }
        }

        private void WebSocketClient_OnMessage(string obj)
        {
            PrintLog("websocket<=" + obj);
            try
            {
                var estimatedData = Json.Parse(obj) as IDictionary<string, object>;
                var validator = AVIMNotice.IsValidLeanCloudProtocol(estimatedData);
                if (validator)
                {
                    var notice = new AVIMNotice(estimatedData);
                    m_NoticeReceived?.Invoke(this, notice);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    PrintLog(ex.InnerException.Source);
                }
                if (ex.Source != null)
                {
                    PrintLog(ex.Source);
                }

                PrintLog(ex.Message);
            }
        }

        /// <summary>
        /// 设定监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="runtimeHook"></param>
        public void SubscribeNoticeReceived(IAVIMListener listener, Func<AVIMNotice, bool> runtimeHook = null)
        {
            this.NoticeReceived += new EventHandler<AVIMNotice>((sender, notice) =>
            {
                var approved = runtimeHook == null ? listener.ProtocolHook(notice) : runtimeHook(notice) && listener.ProtocolHook(notice);
                if (approved)
                {
                    listener.OnNoticeReceived(notice);
                }
            });
        }

        /// <summary>
        /// 初始化配置项
        /// </summary>
        public struct Configuration
        {
            /// <summary>
            /// 签名工厂
            /// </summary>
            public ISignatureFactory SignatureFactory { get; set; }
            /// <summary>
            /// 自定义 WebSocket 客户端
            /// </summary>
            public IWebSocketClient WebSocketClient { get; set; }
            /// <summary>
            /// LeanCloud App Id
            /// </summary>
            public string ApplicationId { get; set; }
            /// <summary>
            /// LeanCloud App Key
            /// </summary>
            public string ApplicationKey { get; set; }

            /// <summary>
            /// 登录的时候告知服务器，本次登录所使用的离线消息策略
            /// </summary>
            public OfflineMessageStrategy OfflineMessageStrategy { get; set; }

            /// <summary>
            /// Gets or sets the realtime server.
            /// </summary>
            /// <value>The realtime server.</value>
            public Uri RealtimeServer { get; set; }

            /// <summary>
            /// Gets or sets the push router server.
            /// </summary>
            /// <value>The push router server.</value>
            public Uri RTMRouter { get; set; }
        }

        /// <summary>
        ///登录时的离线消息下发策略
        /// </summary>
        public enum OfflineMessageStrategy
        {
            /// <summary>
            /// 服务器将所有离线消息一次性在登录之后马上下发下来
            /// </summary>
            Default = 1,

            /// <summary>
            /// 不再下发未读消息，而是下发对话的未读通知，告知客户端有哪些对话处于未读状态
            /// </summary>
            UnreadNotice = 2,

            /// <summary>
            /// ack 和 read 分离, ack 不会清理未读消息
            /// </summary>
            UnreadAck = 3

        }

        /// <summary>
        /// 当前配置
        /// </summary>
        public Configuration CurrentConfiguration { get; internal set; }
        /// <summary>
        /// 初始化实时消息客户端
        /// </summary>
        /// <param name="config"></param>
        public AVRealtime(Configuration config)
        {
            lock (mutex)
            {
                CurrentConfiguration = config;
                if (CurrentConfiguration.WebSocketClient != null)
                {
                    webSocketController = CurrentConfiguration.WebSocketClient;
                }
                if (CurrentConfiguration.SignatureFactory != null)
                {
                    this.SignatureFactory = CurrentConfiguration.SignatureFactory;
                }
                ReconnectOptions = new AVIMReconnectOptions()
                {
                    Interval = 5,
                    Retry = 120
                };
                RegisterMessageType<AVIMMessage>();
                RegisterMessageType<AVIMTypedMessage>();
                RegisterMessageType<AVIMTextMessage>();
            }
        }

        /// <summary>
        /// 初始化实时消息客户端
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="applicationKey"></param>
        public AVRealtime(string applicationId, string applicationKey)
            : this(new Configuration()
            {
                ApplicationId = applicationId,
                ApplicationKey = applicationKey,
            })
        {

        }
        #region websocket log
        internal static Action<string> LogTracker { get; private set; }
        /// <summary>
        /// 打开 WebSocket 日志
        /// </summary>
        /// <param name="trace"></param>
        public static void WebSocketLog(Action<string> trace)
        {
            LogTracker = trace;
        }
        public static void PrintLog(string log)
        {
            if (AVRealtime.LogTracker != null)
            {
                AVRealtime.LogTracker(log);
            }
        }
        #endregion

        /// <summary>
        /// 创建 Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="tag"></param>
        /// <param name="deviceId">设备唯一的 Id。如果是 iOS 设备，需要将 iOS 推送使用的 DeviceToken 作为 deviceId 传入</param>
        /// <param name="secure">是否强制加密 wss 链接</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AVIMClient> CreateClientAsync(string clientId,
            string tag = null,
            string deviceId = null,
            bool secure = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (mutex)
            {
                var client = PreLogIn(clientId, tag, deviceId);

                AVRealtime.PrintLog("begin OpenAsync.");
                return OpenAsync(secure, Subprotocol, cancellationToken).OnSuccess(t =>
                 {
                     AVRealtime.PrintLog("OpenAsync OnSuccess. begin send open sesstion cmd.");

                     var cmd = new SessionCommand()
                        .UA(VersionString)
                        .Tag(tag)
                        .DeviceId(deviceId)
                        .Option("open")
                        .PeerId(clientId);

                     ToggleNotification(true);
                     return AttachSignature(cmd, this.SignatureFactory.CreateConnectSignature(clientId));

                 }).Unwrap().OnSuccess(x =>
                 {
                     var cmd = x.Result;
                     return this.RunCommandAsync(cmd);
                 }).Unwrap().OnSuccess(s =>
                  {
                      AVRealtime.PrintLog("sesstion opened.");
                      state = Status.Online;
                      ToggleHeartBeating(_heartBeatingToggle);
                      var response = s.Result.Item2;
                      if (response.ContainsKey("st"))
                      {
                          _sesstionToken = response["st"] as string;
                      }
                      if (response.ContainsKey("stTtl"))
                      {
                          var stTtl = long.Parse(response["stTtl"].ToString());
                          _sesstionTokenExpire = DateTime.Now.UnixTimeStampSeconds() + stTtl;
                      }
                      return client;
                  });
            }
        }



        /// <summary>
        /// Creates the client async.
        /// </summary>
        /// <returns>The client async.</returns>
        /// <param name="user">User.</param>
        /// <param name="tag">Tag.</param>
        /// <param name="deviceId">Device identifier.</param>
        /// <param name="secure">If set to <c>true</c> secure.</param>
        public Task<AVIMClient> CreateClientAsync(AVUser user = null,
                                                  string tag = null,
                                                  string deviceId = null,
                                                  bool secure = true,
                                                  CancellationToken cancellationToken = default(CancellationToken))
        {
            AVIMClient client = null;
            AVRealtime.PrintLog("begin OpenAsync.");
            return OpenAsync(secure, Subprotocol, cancellationToken).OnSuccess(openTask =>
            {
                AVRealtime.PrintLog("OpenAsync OnSuccess. begin send open sesstion cmd.");
                var userTask = Task.FromResult(user);
                if (user == null)
                    userTask = AVUser.GetCurrentUserAsync();

                return userTask;
            }).Unwrap().OnSuccess(u =>
            {
                var theUser = u.Result;
                return AVCloud.RequestRealtimeSignatureAsync(theUser);
            }).Unwrap().OnSuccess(signTask =>
            {
                var signResult = signTask.Result;
                var clientId = signResult.ClientId;
                var nonce = signResult.Nonce;
                var singnature = signResult.Signature;
                var ts = signResult.Timestamp;

                client = PreLogIn(clientId, tag, deviceId);
                ToggleNotification(true);
                return this.OpenSessionAsync(clientId, tag, deviceId, nonce, ts, singnature, secure);
            }).Unwrap().OnSuccess(s =>
            {
                ToggleHeartBeating(_heartBeatingToggle);

                return client;
            });
        }

        #region pre-login
        internal AVIMClient PreLogIn(string clientId,
            string tag = null,
            string deviceId = null)
        {
            var client = new AVIMClient(clientId, tag, this);
            if (this.OnOfflineMessageReceived != null)
            {
                client.OnOfflineMessageReceived += this.OnOfflineMessageReceived;
            }
            _clientId = clientId;
            _tag = tag;
            _deviceId = deviceId;
            if (_tag != null)
            {
                if (deviceId == null)
                    throw new ArgumentNullException(deviceId, "当 tag 不为空时，必须传入当前设备不变的唯一 id(deviceId)");
            }

            if (string.IsNullOrEmpty(clientId)) throw new Exception("当前 ClientId 为空，无法登录服务器。");

            return client;
        }
        #endregion

        #region after-login


        #endregion

        /// <summary>
        /// 创建 Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="tag"></param>
        /// <param name="deviceId">设备唯一的 Id。如果是 iOS 设备，需要将 iOS 推送使用的 DeviceToken 作为 deviceId 传入</param>
        /// <param name="secure">是否强制加密 wss 链接</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Obsolete("CreateClient is deprecated, please use CreateClientAsync instead.")]
        public Task<AVIMClient> CreateClient(
            string clientId,
            string tag = null,
            string deviceId = null,
            bool secure = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.CreateClientAsync(clientId, tag, deviceId, secure, cancellationToken);
        }

        /// <summary>
        /// websocket 事件的监听的开关
        /// </summary>
        /// <param name="toggle">是否打开</param>
        public void ToggleNotification(bool toggle)
        {
            if (toggle)
            {
                PCLWebsocketClient.OnClosed += WebsocketClient_OnClosed;
                PCLWebsocketClient.OnError += WebsocketClient_OnError;
                PCLWebsocketClient.OnMessage += WebSocketClient_OnMessage;
            }
            else
            {
                PCLWebsocketClient.OnClosed -= WebsocketClient_OnClosed;
                PCLWebsocketClient.OnError -= WebsocketClient_OnError;
                PCLWebsocketClient.OnMessage -= WebSocketClient_OnMessage;
            }
        }

        //public void ToggleOfflineNotification(bool toggle)
        //{
        //    if (toggle)
        //    {
        //        PCLWebsocketClient.OnMessage += WebSocketClient_OnMessage_On_Session_Opening;
        //    }
        //    else
        //    {
        //        PCLWebsocketClient.OnMessage -= WebSocketClient_OnMessage_On_Session_Opening;
        //    }
        //}

        //private void WebSocketClient_OnMessage_On_Session_Opening(string obj)
        //{
        //    AVRealtime.PrintLog("offline<=" + obj);
        //}


        string _beatPacket = "{}";
        bool _heartBeatingToggle = true;
        IAVTimer _heartBeatingTimer;
        /// <summary>
        /// 主动发送心跳包
        /// </summary>
        /// <param name="toggle">是否开启</param>
        /// <param name="interval">时间间隔</param>
        /// <param name="beatPacket">心跳包的内容，默认是个空的 {}</param>
        public void ToggleHeartBeating(bool toggle = true, double interval = 60000, string beatPacket = "{}")
        {
            this._heartBeatingToggle = toggle;
            if (!string.Equals(_beatPacket, beatPacket)) _beatPacket = beatPacket;

            if (_heartBeatingTimer == null)
            {
                _heartBeatingTimer = new AVTimer();
                _heartBeatingTimer.Elapsed += SendHeartBeatingPacket;
                _heartBeatingTimer.Interval = interval;
                _heartBeatingTimer.Start();
                PrintLog("auto heart beating started.");
            }
        }
        void SendHeartBeatingPacket(object sender, TimerEventArgs e)
        {
            PrintLog("auto heart beating ticked by timer.");
#if MONO || UNITY
            Dispatcher.Instance.Post(() =>
            {
                KeepAlive();
            });
#else
            KeepAlive();
#endif
        }

        public void KeepAlive()
        {
            if (this._heartBeatingToggle)
            {
                PCLWebsocketClient.Send(this._beatPacket);
            }
        }
        IAVTimer reconnectTimer;
        bool autoReconnectionStarted = false;

        internal bool sessionConflict = false;


        internal bool CanReconnect
        {
            get
            {
                return !sessionConflict;
            }
        }

        internal void ClearReconnectTimer()
        {
            if (reconnectTimer != null)
            {
                reconnectTimer.Stop();
                reconnectTimer = null;
            }
        }

        /// <summary>
        /// 开始自动重连
        /// </summary>
        public void StartAutoReconnect()
        {
            if (!autoReconnectionStarted && CanReconnect)
            {
                autoReconnectionStarted = true;
                reconnectTimer = new AVTimer();
                reconnectTimer.Interval = this.ReconnectOptions.Interval * 1000;
                reconnectTimer.Elapsed += ReconnectTimer_Elapsed;
                reconnectTimer.Start();
                reconnectTimer.Enabled = true;
            }
        }

        private void ReconnectTimer_Elapsed(object sender, TimerEventArgs e)
        {
            var _timer = sender as AVTimer;
            if (state == Status.Offline)
            {
                if (_timer != null)
                {
                    if (_timer.Executed <= this.ReconnectOptions.Retry && CanReconnect)
                    {
                        AutoReconnect();
                        _timer.Executed += 1;
                    }
                    else
                    {
                        _timer.Stop();
                        _timer = null;
                        autoReconnectionStarted = false;

                        var reconnectFailedArgs = new AVIMReconnectFailedArgs()
                        {
                            ClientId = _clientId,
                            IsAuto = true,
                            SessionToken = _sesstionToken,
                            FailedCode = -1
                        };
                        m_OnReconnectFailed?.Invoke(this, reconnectFailedArgs);
                        state = Status.Offline;
                    }
                }
                else
                {
                    if (CanReconnect)
                        AutoReconnect();
                }
            }
            else if (state == Status.Online)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer = null;
                }

            }
        }

        internal Task LogInAsync(string clientId,
            string tag = null,
            string deviceId = null,
            bool secure = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (mutex)
            {
                var cmd = new SessionCommand()
                           .UA(VersionString)
                           .Tag(tag)
                           .DeviceId(deviceId)
                           .Option("open")
                           .PeerId(clientId);

                var result = AttachSignature(cmd, this.SignatureFactory.CreateConnectSignature(clientId)).OnSuccess(_ =>
                {
                    return RunCommandAsync(cmd);
                }).Unwrap().OnSuccess(t =>
                {
                    AVRealtime.PrintLog("sesstion opened.");
                    if (t.Exception != null)
                    {
                        var imException = t.Exception.InnerException as AVIMException;
                        throw imException;
                    }
                    state = Status.Online;
                    var response = t.Result.Item2;
                    if (response.ContainsKey("st"))
                    {
                        _sesstionToken = response["st"] as string;
                    }
                    if (response.ContainsKey("stTtl"))
                    {
                        var stTtl = long.Parse(response["stTtl"].ToString());
                        _sesstionTokenExpire = DateTime.Now.UnixTimeStampSeconds() + stTtl;
                    }
                    return t.Result;
                });

                return result;
            }

        }

        internal Task OpenSessionAsync(string clientId,
            string tag = null,
            string deviceId = null,
            string nonce = null,
            long timestamp = 0,
            string signature = null,
            bool secure = true)
        {
            var cmd = new SessionCommand()
                .UA(VersionString)
                .Tag(tag)
                .DeviceId(deviceId)
                .Option("open")
                .PeerId(clientId)
                .Argument("n", nonce)
                .Argument("t", timestamp)
                .Argument("s", signature);

            return RunCommandAsync(cmd).OnSuccess(t =>
            {
                AVRealtime.PrintLog("sesstion opened.");
                if (t.Exception != null)
                {
                    var imException = t.Exception.InnerException as AVIMException;
                    throw imException;
                }
                state = Status.Online;
                var response = t.Result.Item2;
                if (response.ContainsKey("st"))
                {
                    _sesstionToken = response["st"] as string;
                }
                if (response.ContainsKey("stTtl"))
                {
                    var stTtl = long.Parse(response["stTtl"].ToString());
                    _sesstionTokenExpire = DateTime.Now.UnixTimeStampSeconds() + stTtl;
                }
                return t.Result;
            });

        }


        /// <summary>
        /// 自动重连
        /// </summary>
        /// <returns></returns>
        public Task AutoReconnect()
        {

            var reconnectingArgs = new AVIMReconnectingEventArgs()
            {
                ClientId = _clientId,
                IsAuto = true,
                SessionToken = _sesstionToken
            };
            m_OnReconnecting?.Invoke(this, reconnectingArgs);

            return OpenAsync(_wss, Subprotocol).ContinueWith(t =>
              {
                  if (t.IsFaulted || t.Exception != null)
                  {
                      state = Status.Reconnecting;
                      var reconnectFailedArgs = new AVIMReconnectFailedArgs()
                      {
                          ClientId = _clientId,
                          IsAuto = true,
                          SessionToken = _sesstionToken,
                          FailedCode = 0// network broken.
                      };
                      m_OnReconnectFailed?.Invoke(this, reconnectFailedArgs);
                      state = Status.Offline;
                      return Task.FromResult(false);
                  }
                  else
                  {
                      if (t.Result)
                      {
                          state = Status.Opened;

                          if (this.IsSesstionTokenExpired)
                          {
                              AVRealtime.PrintLog("sesstion is expired, auto relogin with clientId :" + _clientId);
                              return this.LogInAsync(_clientId, this._tag, this._deviceId, this._secure).OnSuccess(o =>
                              {
                                  ClearReconnectTimer();
                                  return true;
                              });
                          }
                          else
                          {
                              var sessionCMD = new SessionCommand().UA(VersionString).R(1);

                              if (string.IsNullOrEmpty(_tag))
                              {
                                  sessionCMD = sessionCMD.Tag(_tag).SessionToken(this._sesstionToken);
                              }

                              var cmd = sessionCMD.Option("open")
                               .PeerId(_clientId);

                              AVRealtime.PrintLog("reopen sesstion with sesstion token :" + _sesstionToken);
                              return RunCommandAsync(cmd).OnSuccess(c =>
                              {
                                  ClearReconnectTimer();
                                  return true;
                              });
                          }
                      }
                      else return Task.FromResult(false);
                  }

              }).Unwrap().ContinueWith(s =>
               {
                   if (s.IsFaulted || s.Exception != null)
                   {
                       var reconnectFailedArgs = new AVIMReconnectFailedArgs()
                       {
                           ClientId = _clientId,
                           IsAuto = true,
                           SessionToken = _sesstionToken,
                           FailedCode = 1
                       };
                       m_OnReconnectFailed?.Invoke(this, reconnectFailedArgs);
                       state = Status.Offline;
                   }
                   else
                   {
                       if (s.Result)
                       {
                           autoReconnectionStarted = false;
                           reconnectTimer = null;
                           var reconnectedArgs = new AVIMReconnectedEventArgs()
                           {
                               ClientId = _clientId,
                               IsAuto = true,
                               SessionToken = _sesstionToken,
                           };
                           state = Status.Online;
                           m_OnReconnected?.Invoke(this, reconnectedArgs);
                       }
                   }
               });
        }



        #region register IAVIMMessage
        /// <summary>
        /// Registers the subtype of the message.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void RegisterMessageType<T>() where T : IAVIMMessage
        {
            AVIMCorePlugins.Instance.FreeStyleClassingController.RegisterSubclass(typeof(T));
        }
        #endregion

        /// <summary>
        /// open websocket with default configurations.
        /// </summary>
        /// <returns></returns>
        public Task<bool> OpenAsync(bool secure = true)
        {
            return this.OpenAsync(secure, null);
        }

        /// <summary>
        /// fetch wss address from push router and open the websocket connection.
        /// </summary>
        /// <param name="secure">if use ssl encrept</param>
        /// <param name="subprotocol">subprotocol</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> OpenAsync(bool secure, string subprotocol = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == Status.Online)
            {
                AVRealtime.PrintLog("state is Status.Online.");
                return Task.FromResult(true);
            }

            if (CurrentConfiguration.RealtimeServer != null)
            {
                AVRealtime.PrintLog("use configuration websocket url:" + _wss);
                return OpenAsync(CurrentConfiguration.RealtimeServer.ToString(), subprotocol, cancellationToken);
            }
            var routerUrl = CurrentConfiguration.RTMRouter != null ? CurrentConfiguration.RTMRouter.ToString() : null;
            return RouterController.GetAsync(routerUrl, secure, cancellationToken).OnSuccess(_ =>
                {
                    _wss = _.Result.server;
                    state = Status.Connecting;
                    AVRealtime.PrintLog("push router give a url :" + _wss);
                    return OpenAsync(_.Result.server, subprotocol, cancellationToken);
                }).Unwrap();
        }

        /// <summary>
        /// open webcoket connection with cloud.
        /// </summary>
        /// <param name="url">wss address</param>
        /// <param name="subprotocol">subprotocol for websocket</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> OpenAsync(string url, string subprotocol = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (PCLWebsocketClient.IsOpen)
            {
                AVRealtime.PrintLog(url + "is already connectd.");
                return Task.FromResult(true);
            }

            AVRealtime.PrintLog("websocket try to connect url :" + url + "with subprotocol: " + subprotocol);
            AVRealtime.PrintLog(url + " connecting...");
            var tcs = new TaskCompletionSource<bool>();
            Action<string> onError = null;
            onError = ((reason) =>
            {
                PCLWebsocketClient.OnError -= onError;
                tcs.TrySetResult(false);
                tcs.TrySetException(new AVIMException(AVIMException.ErrorCode.FromServer, "try to open websocket at " + url + "failed.The reason is " + reason, null));
            });

            Action onOpend = null;
            onOpend = (() =>
            {
                PCLWebsocketClient.OnError -= onError;
                PCLWebsocketClient.OnOpened -= onOpend;
                tcs.TrySetResult(true);
                AVRealtime.PrintLog(url + " connected.");
            });

            Action<int, string, string> onClosed = null;
            onClosed = (reason, arg0, arg1) =>
            {
                PCLWebsocketClient.OnError -= onError;
                PCLWebsocketClient.OnOpened -= onOpend;
                PCLWebsocketClient.OnClosed -= onClosed;
                tcs.TrySetResult(false);
                tcs.TrySetException(new AVIMException(AVIMException.ErrorCode.FromServer, "try to open websocket at " + url + "failed.The reason is " + reason, null));
            };

            PCLWebsocketClient.OnOpened += onOpend;
            PCLWebsocketClient.OnClosed += onClosed;
            PCLWebsocketClient.OnError += onError;
            PCLWebsocketClient.Open(url, subprotocol);

            return tcs.Task;
        }

        /// <summary>
        /// send websocket command to Realtime server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Tuple<int, IDictionary<string, object>>> RunCommandAsync(AVIMCommand command)
        {
            command.AppId(this.CurrentConfiguration.ApplicationId);
            return this.AVIMCommandRunner.RunCommandAsync(command);
        }

        internal Task<AVIMCommand> AttachSignature(AVIMCommand command, Task<AVIMSignature> SignatureTask)
        {
            AVRealtime.PrintLog("AttachSignature started.");
            var tcs = new TaskCompletionSource<AVIMCommand>();
            if (SignatureTask == null)
            {
                tcs.SetResult(command);
                return tcs.Task;
            }
            return SignatureTask.OnSuccess(_ =>
            {
                if (_.Result != null)
                {
                    var signature = _.Result;
                    command.Argument("t", signature.Timestamp);
                    command.Argument("n", signature.Nonce);
                    command.Argument("s", signature.SignatureContent);
                    AVRealtime.PrintLog("AttachSignature ended.t:" + signature.Timestamp + ";n:" + signature.Nonce + ";s:" + signature.SignatureContent);
                }
                return command;
            });
        }

        private void WebsocketClient_OnError(string obj)
        {
            PrintLog("error:" + obj);
        }

        #region log out and clean event subscribtion
        private void WebsocketClient_OnClosed(int arg1, string arg2, string arg3)
        {
            if (State != Status.Closed)
            {
                state = Status.Offline;
                PrintLog(string.Format("websocket closed with code is {0},reason is {1} and detail is {2}", arg1, arg2, arg3));
                var args = new AVIMDisconnectEventArgs(arg1, arg2, arg3);
                m_OnDisconnected?.Invoke(this, args);

                // 如果断线产生的原因是客户端掉线而不是服务端踢下线，则应该开始自动重连
                if (arg1 == 0)
                {
                    StartAutoReconnect();
                }
            }
        }

        internal void LogOut()
        {
            State = Status.Closed;
            Dispose();
            PCLWebsocketClient.Close();
        }

        internal void Dispose()
        {
            var toggle = false;
            ToggleNotification(toggle);
            ToggleHeartBeating(toggle);
            if (m_NoticeReceived != null)
            {
                foreach (Delegate d in m_NoticeReceived.GetInvocationList())
                {
                    m_NoticeReceived -= (EventHandler<AVIMNotice>)d;
                }
            }
            if (m_OnDisconnected != null)
            {
                foreach (Delegate d in m_OnDisconnected.GetInvocationList())
                {
                    m_OnDisconnected -= (EventHandler<AVIMDisconnectEventArgs>)d;
                }
            }
        }
        #endregion

        static AVRealtime()
        {

#if MONO || UNITY
            versionString = "net-unity/" + Version;
#else
            versionString = "net-portable/" + Version;
#endif
        }

        private static readonly string versionString;
        internal static string VersionString
        {
            get
            {
                return versionString;
            }
        }

        internal static System.Version Version
        {
            get
            {
                AssemblyName assemblyName = new AssemblyName(typeof(AVRealtime).GetTypeInfo().Assembly.FullName);
                return assemblyName.Version;
            }
        }
    }
}
