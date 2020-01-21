using System;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;

namespace Test.Portable {
    public static class Utils {
        internal static AVRealtime NewRealtime() {
            Websockets.Net.WebsocketConnection.Link();
            var appId = "ikGGdRE2YcVOemAaRbgp1xGJ-gzGzoHsz";
            var appKey = "NUKmuRbdAhg1vrb2wexYo1jo";
            var server = "https://ikggdre2.lc-cn-n1-shared.com";
            AVClient.Initialize(appId, appKey, server);
            return new AVRealtime(new AVRealtime.Configuration {
                ApplicationId = appId,
                ApplicationKey = appKey
            });
        }
    }
}
