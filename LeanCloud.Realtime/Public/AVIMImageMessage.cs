using LeanCloud;
using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;
using LeanCloud.Realtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 图像消息
    /// </summary>
    public class AVIMImageMessage : AVIMMessage
    {
        public AVIMImageMessage()
        {

        }

        internal AVFile fileState;
        /// <summary>
        /// 从外链 Url 构建图像消息
        /// </summary>
        /// <returns></returns>
        public static AVIMImageMessage FromUrl(string url)
        {
            AVIMImageMessage imageMessage = new AVIMImageMessage();
            imageMessage.fileState = new AVFile(string.Empty.Random(8), url);
            return imageMessage;
        }
    }
}
