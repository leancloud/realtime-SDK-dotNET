using NUnit.Framework;
using System;
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
    }
}
