using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud.Realtime.Internal;

namespace LeanCloud.Realtime
{
    internal class ConversationUnreadListener : IAVIMListener
    {
        internal class UnreadConversationNotice
        {
            internal readonly object mutex = new object();
            internal IAVIMMessage LastUnreadMessage { get; set; }
            internal string ConvId { get; set; }
            internal int UnreadCount { get; set; }
            internal void AutomicIncrement()
            {
                lock (mutex)
                {
                    UnreadCount++;
                }
            }
        }
        internal readonly object mutex = new object();
        internal static readonly object sMutex = new object();
        internal static float NotifTime;
        internal static HashSet<UnreadConversationNotice> UnreadConversations;
        static ConversationUnreadListener()
        {
            UnreadConversations = new HashSet<UnreadConversationNotice>();
            NotifTime = DateTime.Now.UnixTimeStampSeconds();
        }
        internal static void UpdateNotice(IAVIMMessage message)
        {
            lock (sMutex)
            {
                var convValidators = UnreadConversations.Where(c => c.ConvId == message.ConversationId);
                if (convValidators != null)
                {
                    if (convValidators.Count() > 0)
                    {
                        var currentNotice = convValidators.FirstOrDefault();
                        currentNotice.AutomicIncrement();
                        currentNotice.LastUnreadMessage = message;
                    }
                    else
                    {
                        var currentThread = new UnreadConversationNotice();
                        currentThread.ConvId = message.ConversationId;
                        currentThread.LastUnreadMessage = message;
                        currentThread.AutomicIncrement();
                        UnreadConversations.Add(currentThread);
                    }
                }
            }
        }
        internal static IEnumerable<string> FindAllConvIds()
        {
            lock (sMutex)
            {
                return ConversationUnreadListener.UnreadConversations.Select(c => c.ConvId);
            }
        }

        internal static UnreadConversationNotice Get(string convId)
        {
            lock (sMutex)
            {
                var unreadValidator = ConversationUnreadListener.UnreadConversations.Where(c => c.ConvId == convId);
                if (unreadValidator != null)
                {
                    if (unreadValidator.Count() > 0)
                    {
                        var notice = unreadValidator.FirstOrDefault();
                        return notice;
                    }
                }
                return null;
            }
        }

        public void OnNoticeReceived(AVIMNotice notice)
        {
            lock (mutex)
            {
                if (notice.RawData.ContainsKey("convs"))
                {
                    var unreadRawData = notice.RawData["convs"] as List<object>;
                    if (notice.RawData.ContainsKey("notifTime"))
                    {
                        float.TryParse(notice.RawData["notifTime"].ToString(), out NotifTime);
                    }
                    foreach (var data in unreadRawData)
                    {
                        var dataMap = data as IDictionary<string, object>;
                        if (dataMap != null)
                        {
                            var convId = dataMap["cid"].ToString();
                            var unreadCount = 0;
                            Int32.TryParse(dataMap["unread"].ToString(), out unreadCount);
                            #region restore last message for the conversation
                            var msgStr = dataMap["data"].ToString();
                            var messageObj = AVRealtime.FreeStyleMessageClassingController.Instantiate(msgStr, dataMap);
                            #endregion   
                            var ucn = new UnreadConversationNotice()
                            {
                                ConvId = convId,
                                LastUnreadMessage = messageObj,
                                UnreadCount = unreadCount
                            };
                            UnreadConversations.Add(ucn);
                        }
                    }
                }
            }
        }

        public bool ProtocolHook(AVIMNotice notice)
        {
            return notice.CommandName == "unread";
        }
    }
}
