using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.UnitTest.Mono
{
    [TestFixture()]
    public class CreateNewConversationTest : TestBase
    {
        [Test()]
        public void CreateNewConversation()
        {
            this.realtime.CreateClientAsync("junwu").ContinueWith(t =>
            {
                client = t.Result;
                return client.CreateConversationAsync("houzi");
            }).Unwrap().ContinueWith(s =>
            {
                var conv = s.Result;
            }).Wait();
        }
    }
}
