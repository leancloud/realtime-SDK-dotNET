using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI
{
    public abstract class CompositeCommand: CommandBase
    {
        public CommandBase DecoratedCommnad { get; set; }

        protected CompositeCommand(CommandBase command)
        {
            DecoratedCommnad = command;
        }

        public override void OnExecute(CommandLineApplication app, IConsole console)
        {
            DecoratedCommnad.OnExecute(app, console);
        }
    }
}
