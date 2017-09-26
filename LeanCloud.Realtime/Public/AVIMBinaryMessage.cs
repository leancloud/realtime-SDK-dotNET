using System;
using LeanCloud.Realtime.Internal;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 实时消息的核心基类，它是 Json schema 消息的父类
    /// </summary>
    [AVIMMessageClassName("_AVIMBinaryMessage")]
    public class AVIMBinaryMessage : AVIMMessage
    {
        public AVIMBinaryMessage(byte[] data)
        {
            this.BinaryData = data;
        }

        public byte[] BinaryData { get; set; }

        internal override MessageCommand BeforeSend(MessageCommand cmd)
        {
            var result = base.BeforeSend(cmd);
            result = result.Binary(this.BinaryData);
            return result;
        }
    }
}
