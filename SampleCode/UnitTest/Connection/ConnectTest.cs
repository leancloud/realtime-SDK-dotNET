using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class ConnectTest : TestBase
    {
        string clientId = "wujun";
        [Fact]
        public async Task CreateTestAsync()
        {
            var client = await CNRealtime.CreateClientAsync(clientId);
        }
    }
}
