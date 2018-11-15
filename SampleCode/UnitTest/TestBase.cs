using System;
using LeanCloud;
using Xunit;
using LeanCloud.Realtime;

namespace UnitTest
{
    public abstract class TestBase : IDisposable
    {
        public readonly AVRealtime CNRealtime;
        public readonly string AppId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
        public readonly string AppKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";

        protected TestBase()
        {
            AVClient.HttpLog(Console.WriteLine);
            AVRealtime.WebSocketLog(Console.WriteLine);
            AVClient.Initialize(AppId, AppKey);
            CNRealtime = new AVRealtime(AppId, AppKey);
        }

        public void Dispose()
        {
            // Do "global" teardown here; Called after every test method.
        }
    }
}
