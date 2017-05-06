using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using UnityEngine;


namespace LeanCloud.Realtime.Internal
{

    /// <summary>
    /// LeanCluod Unity Realtime SDK 内置默认的 WebSocketClient
    /// 开发者可以在初始化的时候指定自定义的 WebSocketClient
    /// </summary>
    public class DefaultWebSocketClient : IWebSocketClient
    {
        WebSocket ws;
        public bool IsOpen
        {
            get
            {
                return ws.IsAlive;
            }
        }

        public event Action<int, string, string> OnClosed;
        public event Action<string> OnError;
        public event Action<string> OnLog;
        public event Action<string> OnMessage;
        public event Action OnOpened;

        public void Close()
        {
            ws.CloseAsync();
            ws.OnOpen -= OnOpen;
            ws.OnMessage -= OnWebSokectMessage;
            ws.OnClose -= OnClose;
        }

        public void Open(string url, string protocol = null)
        {
            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += OnWebSokectMessage;
            ws.OnClose += OnClose;
            ws.ConnectAsync();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            AVRealtime.PrintLog("Unity websocket closed without parameters.");
            if (this.OnClosed != null)
                this.OnClosed(e.Code, e.Reason, "");
        }

        private void OnWebSokectMessage(object sender, MessageEventArgs e)
        {
            if (this.OnMessage != null)
                this.OnMessage(e.Data);
        }

        private void OnOpen(object sender, EventArgs e)
        {
            if (this.OnOpened != null)
                this.OnOpened();
        }

        public void Send(string message)
        {
            if (this.IsOpen)
            {
                ws.SendAsync(message, (b) =>
                {

                });
            }

        }
    }
}
