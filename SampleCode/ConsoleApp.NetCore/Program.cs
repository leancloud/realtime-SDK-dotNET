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


            AVIMClient tom = await realtime.CreateClientAsync(clientId, tag: "Mobile", deviceId: "xxxbbbxxx");
            tom.OnSessionClosed += Tom_OnSessionClosed;
            var conversation = await tom.GetConversationAsync("5b83a01a5b90c830ff80aea4");
            var messages = await conversation.QueryMessageAsync(limit: 10);

            var afterMessages = await conversation.QueryMessageAfterAsync(messages.First());
            var earliestMessages = await conversation.QueryMessageFromOldToNewAsync();
            var nextPageMessages = await conversation.QueryMessageAfterAsync(earliestMessages.Last());
            var earliestMessage = await conversation.QueryMessageFromOldToNewAsync(limit: 1);
            var latestMessage = await conversation.QueryMessageAsync(limit: 1);
            var messagesInInterval = await conversation.QueryMessageInIntervalAsync(earliestMessage.FirstOrDefault(), latestMessage.FirstOrDefault());
            Console.ReadKey();
        }

        static void Tom_OnSessionClosed(object sender, AVIMSessionClosedEventArgs e)
        {
        }


        static void Tom_OnMessageModified(object sender, AVIMMessagePatchEventArgs e)
        {
        }


        static void Tom_OnMessageRecalled(object sender, AVIMMessagePatchEventArgs e)
        {
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
                var mentionedAll = e.Message.MentionAll;
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
