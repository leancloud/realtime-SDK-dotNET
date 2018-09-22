using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI.Commands
{
    [Command("lom", Description = "join a conversation and begin to listen on message."),]
    public class OnMessageCommand 
    {
        [Option("--appId", Description = "app id of LeanCloud App")]
        public string AppId { get; set; }

        [Option("--appKey", Description = "app key of LeanCloud App")]
        public string AppKey { get; set; }

        [Option("--convId", Description = "app id of LeanCloud App")]
        public string ConvId { get; set; }

        [Option("--clientId", Description = "app key of LeanCloud App")]
        public string ClientId { get; set; }

        public async Task OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            AVClient.Initialize(AppId, AppKey);
            AVRealtime.WebSocketLog((str) =>
            {
                console.WriteLine(str);
            });
            AVRealtime realtime = new AVRealtime(AppId, AppKey);

            AVIMClient client = await realtime.CreateClientAsync(ClientId);
            client.OnMessageReceived += Client_OnMessageReceived;
            new RealtimeCommand().OnExecute(app, console);
        }

        void Client_OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if(e.Message is AVIMImageMessage imageMessage)
            {
                Console.WriteLine(imageMessage.File.Url);
            }
        }

    }
}
