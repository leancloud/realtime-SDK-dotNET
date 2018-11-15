using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Conversation
{
    public class CreateConversationTest: TestBase
    {
        string clientId = "wujun";

        [Fact]
        public async Task CreateTestAsync()
        {
            var client = await CNRealtime.CreateClientAsync(clientId);
        }
    }
}
