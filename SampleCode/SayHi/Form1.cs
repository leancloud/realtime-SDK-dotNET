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
        string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
        string appkey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
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

            var config = new AVRealtime.Configuration()
            {
                ApplicationId = appId,
                ApplicationKey = appkey,
                OfflineMessageStrategy = AVRealtime.OfflineMessageStrategy.UnreadAck
            };

            realtime = new AVRealtime(config);

            lbx_messages.DisplayMember = "Content";
            lbx_messages.ValueMember = "Id";
            lbx_messages.DataSource = data;

            realtime.OnOfflineMessageReceived += Realtime_OnOfflineMessageReceived;
        }

        private void Realtime_OnOfflineMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            AVRealtime.PrintLog(e.Message.Id);
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
            //var isTransient = ckb_isTransient.Checked;
            conversation = await client.CreateConversationAsync(txb_friend.Text.Trim());
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

        private void btn_Pause_Click(object sender, EventArgs e)
        {
            client.CloseAsync();
        }
    }
}
