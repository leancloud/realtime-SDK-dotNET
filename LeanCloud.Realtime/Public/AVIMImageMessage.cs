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

        public override IAVIMMessage Deserialize(string msgStr)
        {
            var msg = Json.Parse(msgStr) as IDictionary<string, object>;
            var fileData = msg[AVIMProtocol.LCFILE] as IDictionary<string, object>;
            string mimeType = null;
            string url = null;
            string name = null;
            string objId = null;
            IDictionary<string, object> metaData = null;
            if (fileData != null)
            {
                object metaDataObj = null;

                if (fileData.TryGetValue("metaData", out metaDataObj))
                {
                    metaData = metaDataObj as IDictionary<string, object>;
                    object fileNameObj = null;
                    if (metaData != null)
                    {
                        if (metaData.TryGetValue("name", out fileNameObj))
                        {
                            name = fileNameObj.ToString();
                        }
                    }
                    object mimeTypeObj = null;
                    if (metaData != null)
                    {
                        if (metaData.TryGetValue("format", out mimeTypeObj))
                        {
                            if (mimeTypeObj != null)
                                mimeType = mimeTypeObj.ToString();
                        }
                    }
                }

                object objIdObj = null;
                if (fileData.TryGetValue("objId", out objIdObj))
                {
                    if (objIdObj != null)
                        objId = objIdObj.ToString();
                }

                object urlObj = null;
                if (fileData.TryGetValue("url", out urlObj))
                {
                    url = urlObj.ToString();
                }

                File = AVFile.CreateWithData(objId, name, url, metaData);
            }

            return base.Deserialize(msgStr);
        }
    }
}
