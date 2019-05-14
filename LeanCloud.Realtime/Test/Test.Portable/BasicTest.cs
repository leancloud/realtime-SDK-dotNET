using NUnit.Framework;
using System.Threading.Tasks;

namespace Test.Portable {
    [TestFixture]
    public class BasicTest {
        [Test]
        public async Task CreateClient() {
            var realtime = Utils.NewRealtime();
            var client = await realtime.CreateClientAsync("leancloud");
            Assert.AreEqual(client.ClientId, "leancloud");
        }
    }
}
