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
            Console.WriteLine(clientId + " connecting...");
            realtime.CreateClientAsync(clientId).OnSuccess(t =>
            {
                client = t.Result;
                Console.WriteLine(clientId + " connected.");
            });
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

        public void RegisterListener()
        {
            client.OnMessageReceived += (sender, e) => 
            {
                
            };
        }

    }
}
