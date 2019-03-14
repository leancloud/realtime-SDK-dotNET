using System;
using LeanCloud;
using LeanCloud.Realtime;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Test {
    class MainClass {
        static event Action action;

        public static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            AVRealtime.WebSocketLog(Console.WriteLine);
            Websockets.Net.WebsocketConnection.Link();
            Test();
            Console.ReadKey(true);
        }

        static void TestAction() {
            void Func() {
                Console.WriteLine("call func");
            }
            action += Func;
            action += Func;
            action -= Func;
            action?.Invoke();
        }

        static void Test() {

            //var appId = "nb4egfMaDOj6jzqRhBuWpk5m-gzGzoHsz";
            //var appKey = "zJ4aUsCraV6eBE6dGHWYE57z";

            var appId = "Eohx7L4EMfe4xmairXeT7q1w-gzGzoHsz";
            var appKey = "GSBSGpYH9FsRdCss8TGQed0F";

            AVIMConversation conv = null;

            Console.WriteLine($"test at {Thread.CurrentThread.ManagedThreadId}");
            AVClient.Initialize(appId, appKey);
            AVRealtime realtime = new AVRealtime(new AVRealtime.Configuration {
                ApplicationId = appId,
                ApplicationKey = appKey,
                //RealtimeServer = new Uri("wss://rtm51.leancloud.cn/"),
            });
            realtime.CreateClientAsync("leancloud").ContinueWith(t => {
                if (t.IsFaulted) {
                    Console.WriteLine($"create client failed at {Thread.CurrentThread.ManagedThreadId}");
                    throw t.Exception;
                }
                Console.WriteLine($"create client at {Thread.CurrentThread.ManagedThreadId}");
                AVIMClient client = t.Result;
                client.OnMessageReceived += (sender, e) => {
                    Console.WriteLine($"{e.Message.Id} is received at {Thread.CurrentThread.ManagedThreadId}");
                };
                client.OnMessageRecalled += (sender, e) => {
                    Console.WriteLine($"{e.Message} is recall at {Thread.CurrentThread.ManagedThreadId}");
                };
                client.OnMessageUpdated += (sender, e) => {
                    Console.WriteLine($"{e.Message} is updated at {Thread.CurrentThread.ManagedThreadId}");
                };
                realtime.OnDisconnected += (sender, e) => {
                    Console.WriteLine($"{client.ClientId} is disconnected");
                    try {
                        conv.SendTextAsync("I am disconnected");
                    } catch (Exception err) {
                        Console.WriteLine($"send error: {err.Message}");
                    }
                };
                realtime.OnReconnecting += (sender, e) => {
                    Console.WriteLine($"{client.ClientId} is reconnecting");
                };
                realtime.OnReconnected += (sender, e) => {
                    Console.WriteLine($"{client.ClientId} is reconnected");
                    conv.SendTextAsync("I am reconnected");
                };
                //realtime.CreateClient("aaa").ContinueWith(xxx => { 
                //    if (xxx.IsFaulted) {
                //        Console.WriteLine($"create xxx error: {xxx.Exception.Message} at {Thread.CurrentThread.ManagedThreadId}");
                //    } else {
                //        Console.WriteLine($"create xxx at {Thread.CurrentThread.ManagedThreadId}");
                //    }
                //});
                return client.CreateConversationAsync(members: new string[] { "xxx", "zzz" });
            }).Unwrap().ContinueWith(t => {
                if (t.IsFaulted) {
                    throw t.Exception;
                }
                Console.WriteLine($"create conversation at {Thread.CurrentThread.ManagedThreadId}");
                conv = t.Result;
                return conv.SendTextAsync("无码种子");
            }).Unwrap().ContinueWith(t => {
                if (t.IsFaulted) {
                    throw t.Exception;
                }
                Console.WriteLine($"send message at {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("send success");
            });


            //await client.CloseAsync();

            //client = await realtime.CreateClientAsync("leancloud");

            //await client.CloseAsync();

            //var newMsg = new AVIMTextMessage("大家好");

            //var timer = new Timer {
            //    Interval = 1000,
            //};
            //timer.Elapsed += async (sender, e) => {
            //    timer.Stop();

            //    try {
            //        var modifiedMsg = await conv.UpdateAsync(msg, newMsg);
            //        Console.WriteLine($"{modifiedMsg.Id} 修改成功");
            //    } catch (Exception exception) {
            //        Console.WriteLine(exception.Message);
            //    }

            //    //try {
            //    //    var recalledMsg = await conv.RecallAsync(msg);
            //    //    Console.WriteLine($"{recalledMsg.Id} is recalled");
            //    //} catch (Exception exception) {
            //    //    Console.WriteLine(exception.Message);
            //    //}
            //};
            //timer.Start();
        }
    }
}
