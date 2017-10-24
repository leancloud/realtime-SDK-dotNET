using System;
using LeanCloud;
using LeanCloud.Realtime;
using LeanCloud.Storage.Internal;

namespace MockConsole
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Websockets.Net.WebsocketConnection.Link();
            string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
            string appKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
            string clientId = "junwu";


            var helper = new LeanCloudRealtimeHelper(appId, appKey, clientId);

        }
    }

    public class LeanCloudRealtimeHelper
    {
        AVRealtime realtime;
        AVIMClient client;
        AVIMConversation conversation;
        public LeanCloudRealtimeHelper(string appId, string appKey, string clientId)
        {
            AVClient.Initialize(appId, appKey);
            realtime = new AVRealtime(appId, appKey);

            AVRealtime.WebSocketLog(Console.Write);
        }
        public async void CreateClient(string clientId)
        {
            Console.WriteLine(clientId + " connecting...");
            client = await realtime.CreateClientAsync(clientId);
            Console.WriteLine(clientId + " connected.");
        }
        public async void CreateConversation()
        {
            conversation = await client.CreateConversationAsync("Jerry", name: "Tom 和 Jerry 的私聊对话");
        }

        public void SendTextMessage()
        {
            var textMessage = new AVIMTextMessage("人民的名义挺好看的");
            conversation.SendMessageAsync(textMessage);
        }

        public void SendBinaryMessage()
        {
            //         var text = "I love LeanCloud";
            //var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            //var binaryMessage = new AVIMBinaryMessage(textBytes);

            //conversation.SendMessageAsync(binaryMessage);

        }

        public void RegisterListener()
        {

        }

        public void RegisterOfflineListener()
        {
            client.OnOfflineMessageReceived += (sender, e) =>
            {
                Console.WriteLine("received offline message :" + e.Message.GetType());
            };
        }

        public async void CreateChatRoom()
        {
            var chatRoom = await client.CreateConversationAsync(members: new string[] { "Jerry", "Harry" }, isTransient: true);
        }

        public async void CreateGroupChat()
        {
            var groupChat = await client.CreateConversationAsync(members: new string[] { "Jerry", "Harry" });
            await client.InviteAsync(groupChat, "Bob");

            await client.KickAsync(groupChat, "Harry");

            await client.LeftAsync(groupChat);
        }


    }
}
