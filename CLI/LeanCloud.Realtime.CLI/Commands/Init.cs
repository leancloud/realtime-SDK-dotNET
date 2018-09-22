using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI.Commands
{
    [Command("init",
             Description = "init app with appId and appKey"),]
    public class InitCommand : CommandBase
    {
        public override void OnExecute(CommandLineApplication app, IConsole console)
        {
            AVRealtime.WebSocketLog((str) => 
            {
                console.WriteLine(str);
            });
       
            console.WriteLine("app inited.");
        }
    }
}
