using System;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineApplication.Execute<RealtimeCommand>(args);
        } 
    }
}
