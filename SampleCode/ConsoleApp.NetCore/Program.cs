using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;

namespace ConsoleApp.NetCore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
            string appKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
            string clientId = "wujun";

            AVClient.Initialize(appId, appKey);
            AVRealtime realtime = new AVRealtime(appId, appKey);

            AVRealtime.WebSocketLog(Console.WriteLine);

            AVIMClient tom = await realtime.CreateClientAsync(clientId);

            var conversation = await tom.GetConversationAsync("5b83a01a5b90c830ff80aea4");

            //await conversation.SendImageAsync("http://ww3.sinaimg.cn/bmiddle/596b0666gw1ed70eavm5tg20bq06m7wi.gif", "Satomi_Ishihara", "萌妹子一枚", new Dictionary<string, object>
            //{
            //    { "actress","石原里美"}
            //});

            //using (FileStream fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "石原里美就是最美哒.jpg"), FileMode.Open, FileAccess.Read))
            //{
            //    await conversation.SendImageAsync("石原里美就是最美哒.jpg", fileStream);
            //}
            tom.OnMessageReceived += OnMessageReceived;
            Console.ReadKey();
        }

        static void OnKicked(object sender, AVIMOnKickedEventArgs e)
        {
        }


        static void OnMembersLeft(object sender, AVIMOnMembersLeftEventArgs e)
        {
        }


        static void OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if (e.Message is AVIMImageMessage imageMessage)
            {
                Console.WriteLine(imageMessage.File.Url);
            }
        }


        static void OnMembersJoined(object sender, AVIMOnMembersJoinedEventArgs e)
        {

        }

    }
}
