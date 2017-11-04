using System;
using LeanCloud.Realtime.Internal;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 基于二进制数据的消息类型，可以直接发送 Byte 数组
    /// </summary>
    [AVIMMessageClassName("_AVIMBinaryMessage")]
    public class AVIMBinaryMessage : AVIMMessage
    {

        public AVIMBinaryMessage()
        {
        }
        /// <summary>
        /// create new instance of AVIMBinnaryMessage
        /// </summary>
        /// <param name="data"></param>
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
