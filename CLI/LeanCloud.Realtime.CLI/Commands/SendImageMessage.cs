using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI.Commands
{
    [Command("lsm", Description = "join a conversation and begin to listen on message."),]
    public class SendImageMessage
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

            var conversation = await client.GetConversationAsync("5b83a01a5b90c830ff80aea4");

            await conversation.SendImageAsync("http://ww3.sinaimg.cn/bmiddle/596b0666gw1ed70eavm5tg20bq06m7wi.gif", "Satomi_Ishihara", "萌妹子一枚", new Dictionary<string, object>
            {
                { "actress","石原里美"}
            });

            new RealtimeCommand().OnExecute(app, console);
        }
    }
}
