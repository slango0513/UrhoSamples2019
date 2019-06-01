using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UrhoSharp.Viewer.Core.Utils
{
    public static class FsUtils
    {
        public static IEnumerable<string> FindFiles(string folder, Func<string, bool> predicate)
        {
            foreach (var file in Directory.GetFiles(folder))
                if (predicate(file))
                    yield return file;

            foreach (var subDir in Directory.GetDirectories(folder))
                foreach (var item in FindFiles(subDir, predicate))
                    yield return item;
        }

        public static bool SplitPath(string path, string byDirectory, out string left, out string right)
        {
            left = null;
            right = null;
            var separator = Path.DirectorySeparatorChar;
            byDirectory = separator + byDirectory + separator;

            var index = path.LastIndexOf(byDirectory, StringComparison.InvariantCultureIgnoreCase);
            if (index >= 0)
            {
                left = path.Substring(0, index + byDirectory.Length);
                right = path.Substring(index + byDirectory.Length);
                return true;
            }
            return false;
        }

        public static string FindFolder(string path, string suffix)
        {
            var separator = Path.DirectorySeparatorChar;
            var endIndex = path.LastIndexOf(suffix + separator, StringComparison.InvariantCultureIgnoreCase);
            if (endIndex <= 0) return null;
            var beginIndex = path.Substring(0, endIndex + suffix.Length).LastIndexOf(separator);
            if (beginIndex <= 0) return null;
            var result = path.Substring(beginIndex + 1, endIndex - beginIndex + suffix.Length - 1);
            return result;
        }

        public static void CopyEmbeddedResourceTo(this Assembly asm, string resource, string destination = null, bool overwrite = false)
        {
            if (destination == null)
                destination = resource;

            if (File.Exists(destination) && !overwrite)
                return;

            using (Stream input = asm.GetManifestResourceStream(asm.GetName().Name + "." + resource))
            using (Stream output = File.Create(destination))
                input.CopyTo(output);
        }
    }
}
