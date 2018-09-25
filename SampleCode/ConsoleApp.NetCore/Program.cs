using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;
using System.Linq.Expressions;
using System.Linq;

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

            var conv = await tom.GetConversationAsync("5b83a01a5b90c830ff80aea4");

            //await conversation.SendImageAsync("http://ww3.sinaimg.cn/bmiddle/596b0666gw1ed70eavm5tg20bq06m7wi.gif", "Satomi_Ishihara", "萌妹子一枚", new Dictionary<string, object>
            //{
            //    { "actress","石原里美"}
            //});

            //using (FileStream fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "石原里美就是最美哒.jpg"), FileMode.Open, FileAccess.Read))
            //{
            //    var imageMessage = new AVIMImageMessage();
            //    imageMessage.File = new AVFile("San_Francisco.png", fileStream);
            //    imageMessage.TextContent = "发自我的 Windows";
            //    await conversation.SendAsync(imageMessage);
            //}
            //tom.OnMessageReceived += OnMessageReceived;
            //var messages = await conversation.QueryMessageAsync(limit: 10);
            //var oldestMessage = messages.ToList()[0];
            //var imageMessages = await conv.QueryMessageAsync<AVIMImageMessage>();
            ////var conversationBuilder = tom.GetConversationBuilder().SetProperty("type", "private").SetProperty("pinned", true);
            //var conversationBuilder = tom.GetConversationBuilder().SetName("三年二班")
            //                             .AddMember("Bob")
            //                             .AddMember("Harry")
            //                             .AddMember("William");
            //var conversation = await tom.CreateConversationAsync(conversationBuilder);
            //conversation.Name = "xx";
            //await tom.CloseAsync();
            await conv.SendLocationAsync(new AVGeoPoint(31.3753285, 120.9664658));

            var chatRoomBuilder = tom.GetConversationBuilder().SetName("聊天室")
                                         .SetTransient();
            var chatRoom = await tom.CreateConversationAsync(chatRoomBuilder);
            var query = tom.GetQuery();
            await query.GetAsync("");
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
            else if (e.Message is AVIMAudioMessage audioMessage)
            {

            }
            else if (e.Message is AVIMVideoMessage videoMessage)
            {

            }
            else if (e.Message is AVIMFileMessage fileMessage)
            {

            }
            else if (e.Message is AVIMLocationMessage locationMessage)
            {

            }
            else if (e.Message is AVIMTypedMessage baseTypedMessage)
            {

            }// 这里可以继续添加自定义类型的判断条件
        }


        static void OnMembersJoined(object sender, AVIMOnMembersJoinedEventArgs e)
        {

        }

    }
}
