using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LeanCloud.Realtime.Internal
{
    internal static class StringEngine
    {
        internal static string Random(this string str, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static string TempConvId<T>(this IEnumerable<T> objs)
        {
            var base64Strs = 
        }
    }
}
