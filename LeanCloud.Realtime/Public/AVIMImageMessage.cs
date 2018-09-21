using LeanCloud;
using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;
using LeanCloud.Realtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// 图像消息
    /// </summary>
    [AVIMMessageClassName("_AVIMImageMessage")]
    [AVIMTypedMessageTypeInt(-2)]
    public class AVIMImageMessage : AVIMFileMessage
    {

    }



    /// <summary>
    /// File message.
    /// </summary>
    [AVIMMessageClassName("_AVIMFileMessage")]
    [AVIMTypedMessageTypeInt(-6)]
    public class AVIMFileMessage : AVIMMessageDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Realtime.AVIMFileMessage"/> class.
        /// </summary>
        public AVIMFileMessage()
            : base(new AVIMTypedMessage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Realtime.AVIMFileMessage"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public AVIMFileMessage(AVIMTypedMessage message)
            : base(message)
        {

        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public AVFile File { get; set; }

        /// <summary>
        /// Froms the URL.
        /// </summary>
        /// <returns>The URL.</returns>
        /// <param name="url">URL.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FromUrl<T>(string url) where T : AVIMFileMessage, new()
        {
            return FromUrl<T>(string.Empty.Random(8), url, null);
        }

        public static T FromUrl<T>(string fileName, string externalUrl, string textTitle, IDictionary<string, object> customAttributes = null) where T : AVIMFileMessage, new()
        {
            T message = new T();
            message.File = new AVFile(fileName, externalUrl);
            message.TextContent = textTitle;
            message.MergeCustomAttributes(customAttributes);
            return message;
        }

        public static T FromStream<T>(string fileName, Stream data, string mimeType, string textTitle, IDictionary<string, object> metaData, IDictionary<string, object> customAttributes = null) where T : AVIMFileMessage, new()
        {
            T message = new T();
            message.File = new AVFile(fileName, data, mimeType, metaData);
            message.TextContent = textTitle;
            message.MergeCustomAttributes(customAttributes);
            return message;
        }

        /// <summary>
        /// Encodes the decorator.
        /// </summary>
        /// <returns>The decorator.</returns>
        public override IDictionary<string, object> EncodeDecorator()
        {
            if (File.Url == null) throw new InvalidOperationException("File.Url can not be null before it can be sent.");
            File.MetaData["name"] = File.Name;
            File.MetaData["format"] = File.MimeType;
            var fileData = new Dictionary<string, object>
            {
                { "url", File.Url.ToString()},
                { "objId", File.ObjectId },
                { "metaData", File.MetaData }
            };

            return new Dictionary<string, object>
            {
                { AVIMProtocol.LCFILE, fileData }
            };
        }
    }
}
