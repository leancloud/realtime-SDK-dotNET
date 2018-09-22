using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace LeanCloud.Realtime.CLI
{
    public static class Extensions
    {
        public static void LogInput(this IConsole console, params object[] args)
        {
            var separator = $">: ";
            console.LogTag(separator, args);
        }

        public static void LogOutput(this IConsole console, params object[] args)
        {
            var separator = $"<:";
            console.LogTag(separator, args);
            console.WriteLine();
        }

        public static void LogRequest(this IConsole console, params object[] args)
        {
            var separator = $"=>:";
            console.LogTag(separator, args);
            console.WriteLine();
        }

        public static void LogResponse(this IConsole console, params object[] args)
        {
            var separator = $"<=:";
            console.LogTag(separator, args);
            console.WriteLine();
        }

        public static void LogTag(this IConsole console, string separator, params object[] args)
        {
            var result = args.ToList();
            result.Insert(0, separator);
            console.Log(result.ToArray());
        }

        public static void Log(this IConsole console, params object[] args)
        {
            if (args.Length > 0)
            {
                var encoded = args.Select(a => a.ToString());
                var str = string.Join(" ", args.Select(a => a.ToString()));
                console.Write(str);
            }
        }

        public static void LogDictionary(this IConsole console, IDictionary<string, object> data)
        {
            console.WriteLine($"---{nameof(data)}---");
            foreach (var kv in data)
            {
                console.WriteLine($"{kv.Key}={kv.Value}");
            }
            console.WriteLine($"---{nameof(data)}---");
        }
    }
}
