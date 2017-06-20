using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeanCloud.Realtime;
using LeanCloud;

namespace SayHi
{
    public partial class Form1 : Form
    {
        string appId = "3knLr8wGGKUBiXpVAwDnryNT-gzGzoHsz";
        string appkey = "3RpBhjoPXJjVWvPnVmPyFExt";
        AVRealtime realtime;
        AVIMClient client;
        AVIMConversation conversation;
        BindingList<AVIMMessage> data = new BindingList<AVIMMessage>();
        public Form1()
        {
            InitializeComponent();
            Websockets.Net.WebsocketConnection.Link();
            AVRealtime.WebSocketLog(AppendLogs);
            AVClient.Initialize(appId, appkey);
            realtime = new AVRealtime(appId, appkey);
            lbx_messages.DisplayMember = "Content";
            lbx_messages.ValueMember = "Id";
            lbx_messages.DataSource = data;
        }

        private async void btn_logIn_Click(object sender, EventArgs e)
        {
            client = await realtime.CreateClientAsync(txb_clientId.Text.Trim());
            client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if (e.Message is AVIMMessage)
            {
                var baseMeseage = e.Message as AVIMMessage;

                lbx_messages.Invoke((MethodInvoker)(() =>
                {
                    data.Add(baseMeseage);
                    lbx_messages.Refresh();
                }));
            }
        }

        public void AppendLogs(string log)
        {
            txb_logs.Invoke((MethodInvoker)(() =>
            {
                txb_logs.AppendText(log + "\n");
            }));
        }

        private async void btn_create_Click(object sender, EventArgs e)
        {
            var isTransient = ckb_isTransient.Checked;
            conversation = await client.CreateConversationAsync(txb_friend.Text.Trim(), isTransient: isTransient);
        }

        private async void btn_join_Click(object sender, EventArgs e)
        {
            var convId = this.txb_convId.Text.Trim();
            this.conversation = await client.GetConversationAsync(convId, true);
            await client.JoinAsync(this.conversation);

            //AVIMClient avIMClient = await realtime.CreateClientAsync("junwu");
            //avIMClient.OnMessageReceived += Client_OnMessageReceived;
            //AVIMConversation avIMConversation = await avIMClient.GetConversationAsync("5940e71b8fd9c5cf89fb91b7", true);
            //if (avIMConversation != null)
            //{
            //    await avIMConversation.JoinAsync();
            //}
        }
    }
}
