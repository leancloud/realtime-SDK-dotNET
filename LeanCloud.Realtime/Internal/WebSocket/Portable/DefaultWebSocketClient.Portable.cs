using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Websockets;

namespace LeanCloud.Realtime.Internal
{
    /// <summary>
    /// LeanCloud Realtime SDK for .NET Portable 内置默认的 WebSocketClient
    /// </summary>
    public class DefaultWebSocketClient : IWebSocketClient
    {
        internal readonly IWebSocketConnection connection;
        /// <summary>
        /// LeanCluod .NET Realtime SDK 内置默认的 WebSocketClient
        /// 开发者可以在初始化的时候指定自定义的 WebSocketClient
        /// </summary>
        public DefaultWebSocketClient()
        {
            connection = WebSocketFactory.Create();
        }

        public event Action<int, string, string> OnClosed;
        public event Action<string> OnError;
        public event Action<string> OnLog;
        public event Action OnOpened;
        public event Action<string> OnMessage;

        public bool IsOpen
        {
            get
            {
                return connection.IsOpen;
            }
        }

        public void Close()
        {
            if (connection != null)
            {
                connection.Close();
                connection.OnOpened -= Connection_OnOpened;
                connection.OnMessage -= Connection_OnMessage;
                connection.OnClosed -= Connection_OnClosed;
            }
        }

        public void Open(string url, string protocol = null)
        {
            if (connection != null)
            {
                connection.Open(url, protocol);
                connection.OnOpened += Connection_OnOpened;
                connection.OnMessage += Connection_OnMessage;
                connection.OnClosed += Connection_OnClosed;
            }
        }

        private void Connection_OnOpened()
        {
            if (this.OnOpened != null)
                this.OnOpened();
        }

        private void Connection_OnMessage(string obj)
        {
            if (this.OnMessage != null)
                this.OnMessage(obj);
        }

        private void Connection_OnClosed()
        {
            AVRealtime.PrintLog("PCL websocket closed without parameters.");
            if (this.OnClosed != null)
                this.OnClosed(0, "", "");
        }

        public void Send(string message)
        {
            if (connection != null)
            {
                if (this.IsOpen)
                {
                    connection.Send(message);
                }
            }
        }
    }
}
