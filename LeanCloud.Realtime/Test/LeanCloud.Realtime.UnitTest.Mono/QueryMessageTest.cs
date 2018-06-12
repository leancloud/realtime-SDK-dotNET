using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.UnitTest.Mono
{
    [TestFixture()]
    public class QueryMessageTest : TestBase
    {
        [Test()]
        [Timeout(300000)]
        public void QueryMessageBeforeMessageIdTest()
        {
            this.realtime.CreateClientAsync("1001").ContinueWith(t =>
            {
                client = t.Result;
                var conv = AVIMConversation.CreateWithoutData("5b1a40295b90c830ff7f2ec7", client);
                return client.QueryMessageAsync(conv, "h3tXZM3rQa+goG7N6_DaKg");
            }).Unwrap().ContinueWith(s =>
            {
                var messages = s.Result;

            }).Wait();
        }
    }
}
