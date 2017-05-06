using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud.Realtime.Internal;
using LeanCloud;
using LeanCloud.Storage.Internal;
using System.Collections;
using LeanCloud.Core.Internal;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 对话
    /// </summary>
    public class AVIMConversation : IEnumerable<KeyValuePair<string, object>>, IAVObject
    {
        private DateTime? updatedAt;

        private DateTime? createdAt;

        private DateTime? lastMessageAt;

        private string name;

        private AVObject convState;

        internal readonly Object mutex = new Object();
        //private readonly IDictionary<string, object> estimatedData = new Dictionary<string, object>();

        internal AVIMClient _currentClient;

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            lock (mutex)
            {
                return ((IEnumerable<KeyValuePair<string, object>>)convState).GetEnumerator();
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (mutex)
            {
                return ((IEnumerable<KeyValuePair<string, object>>)convState).GetEnumerator();
            }
        }
        virtual public object this[string key]
        {
            get
            {
                return convState[key];
            }
            set
            {
                convState[key] = value;
            }
        }
        public ICollection<string> Keys
        {
            get
            {
                lock (mutex)
                {
                    return convState.Keys;
                }
            }
        }
        public T Get<T>(string key)
        {
            return this.convState.Get<T>(key);
        }
        public bool ContainsKey(string key)
        {
            return this.convState.ContainsKey(key);
        }

        internal IDictionary<string, object> EncodeAttributes()
        {
            var currentOperations = convState.StartSave();
            var jsonToSave = AVObject.ToJSONObjectForSaving(currentOperations);
            return jsonToSave;
        }
        internal void MergeFromPushServer(IDictionary<string, object> json)
        {
            if (json.Keys.Contains("cdate"))
            {
                createdAt = DateTime.Parse(json["cdate"].ToString());
                updatedAt = DateTime.Parse(json["cdate"].ToString());
                json.Remove("cdate");
            }
            if (json.Keys.Contains("lm"))
            {
                var ts = double.Parse(json["lm"].ToString());
                updatedAt = ts.UnixTimeStampSeconds();
                lastMessageAt = ts.UnixTimeStampSeconds();
                json.Remove("lm");
            }
            if (json.Keys.Contains("c"))
            {
                Creator = json["c"].ToString();
                json.Remove("c");
            }
            if (json.Keys.Contains("m"))
            {
                MemberIds = json["m"] as IList<string>;
                json.Remove("m");
            }
            if (json.Keys.Contains("mu"))
            {
                MuteMemberIds = json["mu"] as IList<string>;
                json.Remove("mu");
            }
            if (json.Keys.Contains("tr"))
            {
                IsTransient = bool.Parse(json["tr"].ToString());
                json.Remove("tr");
            }
            if (json.Keys.Contains("sys"))
            {
                IsSystem = bool.Parse(json["sys"].ToString());
                json.Remove("sys");
            }
            if (json.Keys.Contains("cid"))
            {
                ConversationId = json["cid"].ToString();
                json.Remove("cid");
            }

            if (json.Keys.Contains("name"))
            {
                Name = json["name"].ToString();
                json.Remove("name");
            }
        }

        /// <summary>
        /// 当前的AVIMClient，一个对话理论上只存在一个AVIMClient。
        /// </summary>
        public AVIMClient CurrentClient
        {
            get
            {
                if (_currentClient == null) throw new NullReferenceException("当前对话没有关联有效的 AVClient。");
                return _currentClient;
            }
            set
            {
                _currentClient = value;
            }
        }
        /// <summary>
        /// 对话的唯一ID
        /// </summary>
        public string ConversationId { get; internal set; }

        /// <summary>
        /// 对话在全局的唯一的名字
        /// </summary>
        public string Name
        {
            get
            {
                if (convState.ContainsKey("name"))
                {
                    name = this.convState.Get<string>("name");
                }
                return name;
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// 对话中存在的 Client 的 ClientId 列表
        /// </summary>
        public IEnumerable<string> MemberIds { get; internal set; }

        /// <summary>
        /// 对该对话静音的成员列表
        /// <remarks>
        /// 对该对话设置了静音的成员，将不会收到离线消息的推送。
        /// </remarks>
        /// </summary>
        public IEnumerable<string> MuteMemberIds { get; internal set; }

        /// <summary>
        /// 对话的创建者
        /// </summary>
        public string Creator { get; private set; }

        /// <summary>
        /// 是否为聊天室
        /// </summary>
        public bool IsTransient { get; internal set; }

        /// <summary>
        /// 是否系统对话
        /// </summary>
        public bool IsSystem { get; internal set; }

        /// <summary>
        /// 是否是唯一对话
        /// </summary>
        public bool IsUnique { get; internal set; }

        /// <summary>
        /// 对话创建的时间
        /// </summary>
        public DateTime? CreatedAt
        {
            get
            {
                DateTime? nullable;
                lock (this.mutex)
                {
                    nullable = this.createdAt;
                }
                return nullable;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.createdAt = value;
                }
            }
        }

        /// <summary>
        /// 对话更新的时间
        /// </summary>
        public DateTime? UpdatedAt
        {
            get
            {
                DateTime? nullable;
                lock (this.mutex)
                {
                    nullable = this.updatedAt;
                }
                return nullable;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.updatedAt = value;
                }
            }
        }

        /// <summary>
        /// 对话中最后一条消息的时间，可以用此判断对话的最后活跃时间
        /// </summary>
        public DateTime? LastMessageAt
        {
            get
            {
                DateTime? nullable;
                lock (this.mutex)
                {
                    nullable = this.lastMessageAt;
                }
                return nullable;
            }
            private set
            {
                lock (this.mutex)
                {
                    this.lastMessageAt = value;
                }
            }
        }

        /// <summary>
        /// 已知 id，在本地构建一个 AVIMConversation 对象
        /// </summary>
        public AVIMConversation(string id)
            : this(null, null, null, null, null, false, false, null)
        {
            this.ConversationId = id;
        }

        /// <summary>
        /// AVIMConversation Build 驱动器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="creator"></param>
        /// <param name="members"></param>
        /// <param name="muteMembers"></param>
        /// <param name="isTransient"></param>
        /// <param name="isSystem"></param>
        /// <param name="attributes"></param>
        /// <param name="state"></param>
        /// <param name="isUnique"></param>
        internal AVIMConversation(AVIMConversation source = null,
            string name = null,
            string creator = null,
            IEnumerable<string> members = null,
            IEnumerable<string> muteMembers = null,
            bool isTransient = false,
            bool isSystem = false,
            IEnumerable<KeyValuePair<string, object>> attributes = null,
            AVObject state = null,
            bool isUnique = true)
        {
            convState = source != null ? source.convState : new AVObject("_Conversation");


            this.Name = source?.Name;
            this.MemberIds = source?.MemberIds;
            this.Creator = source?.Creator;
            this.MuteMemberIds = source?.MuteMemberIds;

            if (!string.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            if (!string.IsNullOrEmpty(creator))
            {
                this.Creator = creator;
            }
            if (members != null)
            {
                this.MemberIds = members.ToList();
            }
            if (muteMembers != null)
            {
                this.MuteMemberIds = muteMembers.ToList();
            }

            this.IsTransient = isTransient;
            this.IsSystem = isSystem;
            this.IsUnique = isUnique;


            if (state != null)
            {
                convState = state;
                this.ConversationId = state.ObjectId;
                this.CreatedAt = state.CreatedAt;
                this.UpdatedAt = state.UpdatedAt;
                this.MergeMagicFields(state.ToDictionary(x => x.Key, x => x.Value));
            }

            if (attributes != null)
            {
                this.MergeMagicFields(attributes.ToDictionary(x => x.Key, x => x.Value));
            }

        }

        /// <summary>
        /// 从本地构建一个对话
        /// </summary>
        /// <param name="convId">对话的 objectId</param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static AVIMConversation CreateWithoutData(string convId, AVIMClient client)
        {
            return new AVIMConversation()
            {
                ConversationId = convId,
                CurrentClient = client
            };
        }

        public static AVIMConversation CreateWithData(IEnumerable<KeyValuePair<string, object>> magicFields, AVIMClient client)
        {
            if (magicFields is AVObject)
            {
                return new AVIMConversation(state: (AVObject)magicFields)
                {
                    CurrentClient = client
                };
            }
            return new AVIMConversation(attributes: magicFields)
            {
                CurrentClient = client
            };
        }

        #region save to cloud
        /// <summary>
        /// 将修改保存到云端
        /// </summary>
        /// <returns></returns>
        public Task SaveAsync()
        {
            var cmd = new ConversationCommand()
               .Generate(this);

            var convCmd = cmd.Option("update")
                .PeerId(this.CurrentClient.ClientId);

            return AVRealtime.AVIMCommandRunner.RunCommandAsync(convCmd);

        }
        #endregion

        #region send message
        /// <summary>
        /// 向该对话发送消息。
        /// </summary>
        /// <param name="avMessage">消息体</param>
        /// <param name="receipt">是否需要送达回执</param>
        /// <param name="transient">是否是暂态消息，暂态消息不返回送达回执(ack)，不保留离线消息，不触发离线推送</param>
        /// <param name="priority">消息等级，默认是1，可选值还有 2 ，3</param>
        /// <param name="will">标记该消息是否为下线通知消息</param>
        /// <param name="pushData">如果消息的接收者已经下线了，这个字段的内容就会被离线推送到接收者
        /// <remarks>例如，一张图片消息的离线消息内容可以类似于：[您收到一条图片消息，点击查看] 这样的推送内容，参照微信的做法</remarks>
        /// </param>
        /// <returns></returns>
        public Task<IAVIMMessage> SendMessageAsync(IAVIMMessage avMessage,
            bool receipt = true,
            bool transient = false,
            int priority = 1,
            bool will = false,
            IDictionary<string, object> pushData = null)
        {
            return this.SendMessageAsync(avMessage, new AVIMSendOptions()
            {
                Receipt = receipt,
                Transient = transient,
                Priority = priority,
                Will = will,
                PushData = pushData
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="avMessage">消息体</param>
        /// <param name="options">消息的发送选项，包含了一些特殊的标记<see cref="AVIMSendOptions"/></param>
        /// <returns></returns>
        public Task<IAVIMMessage> SendMessageAsync(IAVIMMessage avMessage, AVIMSendOptions options)
        {
            if (this.CurrentClient == null) throw new Exception("当前对话未指定有效 AVIMClient，无法发送消息。");
            if (this.CurrentClient.LinkedRealtime.State != AVRealtime.Status.Online) throw new Exception("未能连接到服务器，无法发送消息。");
            return this.CurrentClient.SendMessageAsync(this, avMessage, options);
        }
        #endregion

        #region message listener and event notify
        public void RegisterListener(IAVIMListener listener)
        {
            this.CurrentClient.RegisterListener(listener, this.ConversationIdHook);
        }

        internal bool ConversationIdHook(AVIMNotice notice)
        {
            if (!notice.RawData.ContainsKey("cid")) return false;
            return notice.RawData["cid"].ToString() == this.ConversationId;
        }
        #endregion

        #region mute && unmute
        /// <summary>
        /// 当前用户针对对话做静音操作
        /// </summary>
        /// <returns></returns>
        public Task MuteAsync()
        {
            return this.CurrentClient.MuteConversationAsync(this);
        }
        /// <summary>
        /// 当前用户取消对话的静音，恢复该对话的离线消息推送
        /// </summary>
        /// <returns></returns>
        public Task UnmuteAsync()
        {
            return this.CurrentClient.UnmuteConversationAsync(this);
        }
        #endregion

        #region 成员操作相关接口
        /// <summary>
        /// CurrentClient 主动加入到对话中
        /// <para>签名操作</para>
        /// </summary>
        /// <returns></returns>
        public Task JoinAsync()
        {
            return AddMembersAsync(CurrentClient.ClientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Task AddMembersAsync(string clientId = null, IEnumerable<string> clientIds = null)
        {
            return this.CurrentClient.InviteAsync(this, clientId, clientIds);
        }

        public Task RemoveMembersAsync(string clientId = null, IEnumerable<string> clientIds = null)
        {
            return this.CurrentClient.KickAsync(this, clientId, clientIds);
        }

        public Task QuitAsync()
        {
            return RemoveMembersAsync(CurrentClient.ClientId);
        }
        #endregion

        #region load message history
        /// <summary>
        /// 获取当前对话的历史消息
        /// <remarks>不支持聊天室（暂态对话）</remarks>
        /// </summary>
        /// <param name="beforeMessageId">从 beforeMessageId 开始向前查询（和 beforeTimeStampPoint 共同使用，为防止某毫秒时刻有重复消息）</param>
        /// <param name="afterMessageId"> 截止到某个 afterMessageId (不包含)</param>
        /// <param name="beforeTimeStampPoint">从 beforeTimeStampPoint 开始向前查询</param>
        /// <param name="afterTimeStampPoint">拉取截止到 afterTimeStampPoint 时间戳（不包含）</param>
        /// <param name="limit">获取的消息数量</param>
        /// <returns></returns>
        public Task<IEnumerable<IAVIMMessage>> QueryMessageAsync(
           string beforeMessageId = null,
           string afterMessageId = null,
           DateTime? beforeTimeStampPoint = null,
           DateTime? afterTimeStampPoint = null,
           int limit = 20)
        {
            return this.CurrentClient.QueryMessageAsync(this, beforeMessageId, afterMessageId, beforeTimeStampPoint, afterTimeStampPoint, limit);
        }


        #endregion

        #region 字典与对象之间的转换
        internal virtual void MergeMagicFields(IDictionary<String, Object> data)
        {
            lock (this.mutex)
            {
                if (data.ContainsKey("objectId"))
                {
                    this.ConversationId = (data["objectId"] as String);
                    data.Remove("objectId");
                }
                if (data.ContainsKey("createdAt"))
                {
                    this.CreatedAt = AVDecoder.ParseDate(data["createdAt"] as string);
                    data.Remove("createdAt");
                }
                if (data.ContainsKey("updatedAt"))
                {
                    this.updatedAt = AVDecoder.ParseDate(data["updatedAt"] as string);
                    data.Remove("updatedAt");
                }
                if (data.ContainsKey("name"))
                {
                    this.Name = (data["name"] as String);
                    data.Remove("name");
                }
                if (data.ContainsKey("lm"))
                {
                    this.LastMessageAt = AVDecoder.Instance.Decode(data["lm"]) as DateTime?;
                    data.Remove("lm");
                }
                if (data.ContainsKey("m"))
                {
                    this.MemberIds = AVDecoder.Instance.DecodeList<string>(data["m"]);
                    data.Remove("m");
                }
                if (data.ContainsKey("mu"))
                {
                    this.MuteMemberIds = AVDecoder.Instance.DecodeList<string>(data["mu"]);
                    data.Remove("mu");
                }
                if (data.ContainsKey("c"))
                {
                    this.Creator = data["c"].ToString();
                    data.Remove("c");
                }
                if (data.ContainsKey("unique"))
                {
                    if (data["unique"] is bool)
                    {
                        this.IsUnique = (bool)data["unique"];
                    }
                    data.Remove("unique");
                }
                foreach (var kv in data)
                {
                    this[kv.Key] = kv.Value;
                }
            }
        }
        #endregion
    }
}
