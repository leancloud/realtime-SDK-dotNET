using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI
{
    public abstract class CommandBase
    {
        [Option("--appId", Description = "app id of LeanCloud App")]
        public string AppId { get; set; }

        [Option("--appKey", Description = "app key of LeanCloud App")]
        public string AppKey { get; set; }

        [Option("--convId", Description = "app id of LeanCloud App")]
        public string ConvId { get; set; }

        [Option("--clientId", Description = "app key of LeanCloud App")]
        public string ClientId { get; set; }

        public abstract void OnExecute(CommandLineApplication app, IConsole console);
    }
}
