using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 消息接口
    /// <para>所有消息必须实现这个接口</para>
    /// </summary>
    public interface IAVIMMessage
    {
        string Serialize();

        bool Validate(string msgStr);

        IAVIMMessage Deserialize(string msgStr);

        string ConversationId { get; set; }
        string FromClientId { get; set; }
        string Id { get; set; }
        long ServerTimestamp { get; set; }
        long RcpTimestamp { get; set; }
    }
}
