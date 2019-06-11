using System;
using LeanCloud;
using LeanCloud.Realtime;

namespace MemoryCheck {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            Test();

            string cmd;
            while ((cmd = Console.ReadLine()) != null) { 
                if ("gc".Equals(cmd)) {
                    GC.Collect();
                    Console.WriteLine("gc done");
                }
            }
        }

        static async void Test() {
            AVRealtime.WebSocketLog(Console.WriteLine);

            Websockets.Net.WebsocketConnection.Link();
            var appId = "Eohx7L4EMfe4xmairXeT7q1w-gzGzoHsz";
            var appKey = "GSBSGpYH9FsRdCss8TGQed0F";
            AVClient.Initialize(appId, appKey);
            var realtime = new AVRealtime(appId, appKey);
            var client = await realtime.CreateClientAsync("lean");
            client.OnMessageReceived += (sender, e) => {
                var msg = e.Message;
                if (msg is AVIMMessage) {
                    Console.WriteLine((msg as AVIMMessage).Content);
                }
            };
            var conv = await client.CreateConversationAsync("cloud");
            Console.WriteLine($"conversation id: {conv.ConversationId}");
        }
    }
}
