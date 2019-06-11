using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LeanCloud.Realtime;

namespace Test.Portable {
    [TestFixture]
    public class MessageTest {
        [Test]
        public async Task ModifyMessage() {
            var mre = new ManualResetEvent(false);
            string msgId = null;
            var r = Utils.NewRealtime();
            var c = await r.CreateClientAsync("mt0_c0");
            c.OnMessageUpdated += (sender, e) => {
                Assert.AreEqual(msgId, e.Message.Id);
            };
            var chatroom = await c.CreateConversationAsync("leancloud");
            var textMsg = new AVIMTextMessage("hello");
            var msg = await chatroom.SendMessageAsync(textMsg);
            var newMsg = new AVIMTextMessage("hi");
            msg = await chatroom.UpdateAsync(msg, newMsg);
            msgId = msg.Id;
        }

        [Test]
        public async Task CustomAttr() {
            var mre = new ManualResetEvent(false);
            var r0 = Utils.NewRealtime();
            var c0 = await r0.CreateClientAsync("mt1_c0");
            var r1 = Utils.NewRealtime();
            var c1 = await r1.CreateClientAsync("mt1_c1");

            c1.OnMessageReceived += (sender, e) => {
                if (e.Message is AVIMTextMessage) {
                    var textMsg = e.Message as AVIMTextMessage;
                    var world = textMsg["hello"] as string;
                    Console.WriteLine(world);
                    Assert.AreEqual(world, "world");
                    mre.Set();
                }
            };

            var conv = await c0.CreateConversationAsync(new List<string> { "mt1_c1" });
            var msg = new AVIMTextMessage("the message with custom attributes");
            msg["hello"] = "world";
            await conv.SendMessageAsync(msg);
            mre.WaitOne();
        }
    }
}
