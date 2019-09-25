using System;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;

namespace Test.Portable {
    public static class Utils {
        internal static AVRealtime NewRealtime() {
            Websockets.Net.WebsocketConnection.Link();
            var appId = "Eohx7L4EMfe4xmairXeT7q1w-gzGzoHsz";
            var appKey = "GSBSGpYH9FsRdCss8TGQed0F";
            var server = "https://eohx7l4e.lc-cn-n1-shared.com";
            AVClient.Initialize(appId, appKey, server);
            return new AVRealtime(new AVRealtime.Configuration {
                ApplicationId = appId,
                ApplicationKey = appKey
            });
        }
    }
}
