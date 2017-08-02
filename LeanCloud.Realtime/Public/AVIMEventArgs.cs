using System;
using System.Collections;
using System.Collections.Generic;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 
    /// </summary>
    public class AVIMEventArgs : EventArgs
    {
        public AVIMEventArgs()
        {

        }
        public AVIMException.ErrorCode ErrorCode { get; internal set; }
        /// <summary>
        /// LeanCloud 服务端发往客户端消息通知
        /// </summary>
        public string Message { get; set; }
    }

    public class AVIMDisconnectEventArgs : EventArgs
    {
        public int Code { get; private set; }

        public string Reason { get; private set; }

        public string Detail { get; private set; }

        public AVIMDisconnectEventArgs()
        {

        }
        public AVIMDisconnectEventArgs(int _code,string _reason, string _detail)
        {
            this.Code = _code;
            this.Reason = _reason;
            this.Detail = _detail;
        }
    }

    /// <summary>
    /// 开始重连之后触发正在重连的事件通知，提供给监听者的事件参数
    /// </summary>
    public class AVIMReconnectingEventArgs : EventArgs
    {
        /// <summary>
        ///  是否由 SDK 内部机制启动的自动重连
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 重连的 client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 重连时使用的 SessionToken
        /// </summary>
        public string SessionToken { get; set; }
    }

    /// <summary>
    /// 重连之后的事件回调，有可能失败，也有可能成功
    /// </summary>
    public class AVIMReconnectedEventArgs : EventArgs
    {
        /// <summary>
        ///  是否由 SDK 内部机制启动的自动重连
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 重连的 client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 重连时使用的 SessionToken
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// 重连是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }


    public class AVIMMessageEventArgs : EventArgs
    {
        public AVIMMessageEventArgs(IAVIMMessage iMessage)
        {
            Message = iMessage;
        }
        public IAVIMMessage Message { get; internal set; }
    }

    public class AVIMTextMessageEventArgs : EventArgs
    {
        public AVIMTextMessageEventArgs(AVIMTextMessage raw)
        {
            TextMessage = raw;
        }
        public AVIMTextMessage TextMessage { get; internal set; }
    }

    /// <summary>
    /// 当对话中有人加入时，触发 <seealso cref="AVIMMembersJoinListener.OnMembersJoined"/> 时所携带的事件参数
    /// </summary>
    public class AVIMOnMembersJoinedEventArgs : EventArgs
    {
        /// <summary>
        /// 加入到对话的 Client Id(s)
        /// </summary>
        public IEnumerable<string> JoinedMembers { get; internal set; }

        /// <summary>
        /// 邀请的操作人
        /// </summary>
        public string InvitedBy { get; internal set; }

        /// <summary>
        /// 此次操作针对的对话 Id
        /// </summary>
        public string ConversationId { get; internal set; }
    }

    /// <summary>
    /// 当对话中有人加入时，触发 AVIMMembersJoinListener<seealso cref="AVIMMembersLeftListener.OnMembersLeft"/> 时所携带的事件参数
    /// </summary>
    public class AVIMOnMembersLeftEventArgs : EventArgs
    {
        /// <summary>
        /// 离开对话的 Client Id(s)
        /// </summary>
        public IEnumerable<string> LeftMembers { get; internal set; }

        /// <summary>
        /// 踢出的操作人
        /// </summary>
        public string KickedBy { get; internal set; }

        /// <summary>
        /// 此次操作针对的对话 Id
        /// </summary>
        public string ConversationId { get; internal set; }
    }
    /// <summary>
    /// 当前用户被邀请加入到对话
    /// </summary>
    public class AVIMOnInvitedEventArgs : EventArgs
    {
        /// <summary>
        /// 踢出的操作人
        /// </summary>
        public string InvitedBy { get; internal set; }

        /// <summary>
        /// 此次操作针对的对话 Id
        /// </summary>
        public string ConversationId { get; internal set; }
    }

    /// <summary>
    /// 当前用户被他人从对话中踢出
    /// </summary>
    public class AVIMOnKickedEventArgs : EventArgs
    {
        /// <summary>
        /// 踢出的操作人
        /// </summary>
        public string KickedBy { get; internal set; }

        /// <summary>
        /// 此次操作针对的对话 Id
        /// </summary>
        public string ConversationId { get; internal set; }
    }

    public class AVIMSessionClosedEventArgs : EventArgs
    {
        public int Code { get; internal set; }

        public string Reason { get; internal set; }

        public string Detail { get; internal set; }
    }
}
