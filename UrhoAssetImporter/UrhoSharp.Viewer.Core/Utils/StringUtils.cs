using System;
using System.Linq;

namespace UrhoSharp.Viewer.Core.Utils
{
    public static class StringUtils
    {
        public static bool IsAnyOf(this string str, params string[] args)
        {
            var lowerStr = str.ToLowerInvariant();
            foreach (var item in args)
            {
                if (item.ToLowerInvariant() == lowerStr)
                    return true;
            }
            return false;
        }

        public static string[] WordWrap(this string str, int maxLength)
        {
            int chars = 0;
            return str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (chars += w.Length + 1) / maxLength)
                .Select(g => CutIfLonger(string.Join(" ", g), maxLength, true))
                .ToArray();
        }

        public static string CutIfLonger(this string str, int length, bool useThreedots = false)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= length)
                return str;

            if (useThreedots)
                return str.Substring(0, length - 3) + "...";
            return str.Substring(0, length);
        }
    }
}
