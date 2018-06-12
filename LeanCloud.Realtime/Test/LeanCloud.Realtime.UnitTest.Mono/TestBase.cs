using NUnit.Framework;
using System;
using LeanCloud;

namespace LeanCloud.Realtime.UnitTest.Mono
{
    [TestFixture()]
    public class TestBase
    {
        public AVRealtime realtime;
        public AVIMClient client;
        [SetUp()]
        public virtual void SetUp()
        {
            string appId = "cfpwdlo41ujzbozw8iazd8ascbpoirt2q01c4adsrlntpvxr";
            string appkey = "lmar9d608v4qi8rvc53zqir106h0j6nnyms7brs9m082lnl7";

            //string appId = "EB23wEzd6WPhGlwjMVgEPxg6-gzGzoHsz";
            //string appkey = "6jEGd98CIOUyH6LQrotQSNVb";

            Websockets.Net.WebsocketConnection.Link();

            var coreConfig = new AVClient.Configuration
            {
                ApplicationId = appId,
                ApplicationKey = appId,
            };
            AVClient.Initialize(coreConfig);
            AVClient.HttpLog(AppendLogs);

            var realtimeConfig = new AVRealtime.Configuration()
            {
                ApplicationId = appId,
                ApplicationKey = appkey,
                RealtimeServer = new Uri("wss://rtm51.leancloud.cn"),
                OfflineMessageStrategy = AVRealtime.OfflineMessageStrategy.Default
            };

            AVRealtime.WebSocketLog(AppendLogs);

            realtime = new AVRealtime(realtimeConfig);
        }

        public void AppendLogs(string logMessage)
        {
            System.Diagnostics.Debug.WriteLine(logMessage);
        }
    }
}
