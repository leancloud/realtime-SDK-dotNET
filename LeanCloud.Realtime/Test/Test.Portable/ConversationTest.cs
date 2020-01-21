using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;

namespace Test.Portable {
    [TestFixture]
    public class ConversationTest {
        [SetUp]
        public void SetUp() {
            AVClient.HttpLog(Console.WriteLine);
            AVRealtime.WebSocketLog(Console.WriteLine);
        }

        [Test]
        public async Task ChatRoom() {
            var r = Utils.NewRealtime();
            var c = await r.CreateClientAsync("ct0_c0");
            var chatroom = await c.CreateChatRoomAsync("ct0_conv0");
            Assert.AreEqual(chatroom.Name, "ct0_conv0");
        }

        [Test]
        public async Task GetConversation() {
            var r = Utils.NewRealtime();
            var c = await r.CreateClientAsync("ct0_c0");
            var conv = await c.GetConversationAsync("5e25b614fbf47f0067298de1");
            conv["hello"] = "haha";
            await conv.SaveAsync();
        }

        [Test]
        public async Task Invite() {
            var mre = new ManualResetEvent(false);
            var r0 = Utils.NewRealtime();
            var c0 = await r0.CreateClientAsync("ct1_c0");
            var r1 = Utils.NewRealtime();
            var c1 = await r1.CreateClientAsync("ct1_c1");

            c1.OnInvited += (sender, e) => {
                Console.WriteLine($"I am invited by {e.InvitedBy} in {e.ConversationId}");
                mre.Set();
            };

            var conv = await c0.CreateConversationAsync(new List<string> { "xxx" }, "conv");
            var invite = c0.InviteAsync(conv, "ct1_c1");
            mre.WaitOne();
        }

        [Test]
        public async Task Kick() {
            var mre = new ManualResetEvent(false);
            var r0 = Utils.NewRealtime();
            var c0 = await r0.CreateClientAsync("ct2_c0");
            var r1 = Utils.NewRealtime();
            var c1 = await r1.CreateClientAsync("ct2_c1");

            c1.OnKicked += (sender, e) => {
                Console.WriteLine($"I am kicked by {e.KickedBy} in {e.ConversationId}");
                mre.Set();
            };

            var conv = await c0.CreateConversationAsync(new List<string> { "ct2_c1" }, "ct2_conv0");
            await c0.KickAsync(conv, "ct2_c1");

            mre.WaitOne();
        }

        [Test]
        public async Task TempotaryConversation() {
            var r = Utils.NewRealtime();
            var c = await r.CreateClientAsync("ct3");
            var tempConv = await c.CreateTemporaryConversationAsync();
            Assert.AreEqual(tempConv.ConversationId.StartsWith("_tmp:", StringComparison.Ordinal), true);
            await tempConv.SendTextAsync("hello, leancloud");
        }
    }
}
