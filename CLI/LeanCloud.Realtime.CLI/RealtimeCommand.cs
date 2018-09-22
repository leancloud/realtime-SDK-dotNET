using System;
using System.Threading.Tasks;
using LeanCloud.Realtime.CLI.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI
{
    [Command(FullName = "av",
             AllowArgumentSeparator = true,
             ShowInHelpText = true,
             ThrowOnUnexpectedArgument = true)]
    [Subcommand("init",typeof(InitCommand))]
    [Subcommand("lom", typeof(OnMessageCommand))]
    [Subcommand("lsm", typeof(SendImageMessage))]
    public class RealtimeCommand : CommandBase
    {
        public override void OnExecute(CommandLineApplication app, IConsole console)
        {
            var q = false;
            console.LogInput();
            while (!q)
            {
                var userInput = Console.ReadLine();
                if (userInput.Equals(nameof(q)))
                {
                    q = true;
                }
                var args = userInput.Split(' ');
                var cmd = args[0];
                console.LogOutput(args);
                console.LogInput();
            }
        }
    }
}
