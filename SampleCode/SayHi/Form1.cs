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
    public class MessageVM
    {
        public AVIMMessage MetaData { get; set; }

        public string Id
        {
            get
            {
                return MetaData.Id;
            }
        }

        public string Content
        {
            get
            {
                return MetaData.FromClientId + ": " + MetaData.Content;
            }
        }
    }
    public partial class Form1 : Form
    {
        string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
        string appkey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
        AVRealtime realtime;
        AVIMClient client;
        AVIMConversation conversation;
        BindingList<MessageVM> data = new BindingList<MessageVM>();

        MessageVM selected;
        public Form1()
        {
            InitializeComponent();

            var coreConfig = new AVClient.Configuration
            {
                ApplicationId = appId,
                ApplicationKey = appId,
            };
            AVClient.Initialize(coreConfig);
            AVClient.HttpLog(AppendLogs);

            var realtimeConfig = new AVRealtime.Configuration()
            {
                ApplicationId = appId,
                ApplicationKey = appkey,
                OfflineMessageStrategy = AVRealtime.OfflineMessageStrategy.UnreadAck,
            };
            Websockets.Net.WebsocketConnection.Link();
            AVRealtime.WebSocketLog(AppendLogs);

            realtime = new AVRealtime(realtimeConfig);

            lbx_messages.DisplayMember = "Content";
            lbx_messages.ValueMember = "Id";
            lbx_messages.DataSource = data;

            realtime.OnOfflineMessageReceived += Realtime_OnOfflineMessageReceived;

        }

        private void Realtime_OnOfflineMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            AVRealtime.PrintLog("offline message received:" + e.Message.Id);
        }

        private async void btn_logIn_Click(object sender, EventArgs e)
        {
            client = await realtime.CreateClientAsync(clientId: txb_clientId.Text.Trim());
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnMessageRecalled += Client_OnMessageRecalled;
            client.OnMessageModified += Client_OnMessageModified;
        }

        private void Client_OnMessageRecalled(object sender, AVIMMessagePatchEventArgs e)
        {
            var list = e.Messages.ToList();
            Console.WriteLine(list[0].Id + " has been recalled.");
        }

        private vood Client_OnMessageModified(object sender, AVIMMessagePatchEventArgs e))
        {
            var list = e.Messages.ToList();
            Console.WriteLine(list[0].Id + " has been modified.");
        }

        private void Client_OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if (e.Message is AVIMMessage)
            {
                var baseMeseage = e.Message as AVIMMessage;

                lbx_messages.Invoke((MethodInvoker)(() =>
                {
                    data.Add(new MessageVM() { MetaData = baseMeseage });
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
        }

        private void btn_Pause_Click(object sender, EventArgs e)
        {
            client.CloseAsync();
        }

        private async void btn_Send_Click(object sender, EventArgs e)
        {
            var text = txb_InputMessage.Text != null ? txb_InputMessage.Text : "no text";
            var textMessage = new AVIMTextMessage(text);
            await this.client.SendMessageAsync(this.conversation, textMessage);
            lbx_messages.Invoke((MethodInvoker)(() =>
            {
                data.Add(new MessageVM() { MetaData = textMessage });
                lbx_messages.Refresh();
            }));
        }

        private void btn_markAllAsRead_Click(object sender, EventArgs e)
        {
            this.client.ReadAllAsync();
        }

        private async void btn_Recall_Click(object sender, EventArgs e)
        {
            if (this.selected != null)
            {
                if (this.selected.MetaData.FromClientId == this.client.ClientId)
                {
                    await this.client.RecallAsync(this.selected.MetaData);
                }
            }
        }

        private void lbx_messages_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine(e);

            var lbx = sender as ListBox;
            var message = lbx.SelectedItem as MessageVM;
            if (message != null)
            {
                this.selected = message;
            }
        }

        private async void btn_Modify_Click(object sender, EventArgs e)
        {
            if (this.selected.MetaData is AVIMTextMessage)
            {
                var textMessage = (AVIMTextMessage)this.selected.MetaData;
                textMessage.TextContent = "fixed content";
            }
            await this.client.ModifyAysnc(this.selected.MetaData);
        }
    }
}
