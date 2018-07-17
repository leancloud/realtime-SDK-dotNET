using System;
using System.Threading.Tasks;
using LeanCloud;
using LeanCloud.Realtime;

namespace ConsoleApp.NetCore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
            string appKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
            string clientId = "junwu";

            AVClient.Initialize(appId, appKey);
            AVRealtime realtime = new AVRealtime(appId, appKey);

            AVRealtime.WebSocketLog(Console.WriteLine);

            AVIMClient client = await realtime.CreateClientAsync(clientId);
            Console.ReadKey();
        }
    }
}
