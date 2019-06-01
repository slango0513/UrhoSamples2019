using System.Collections.Generic;
using System.IO;

namespace UrhoSharp.Viewer.Wpf
{
    public class SolutionItem
    {
        public bool IsFolder { get; }
        public string Path { get; }
        public string Name { get; }
        public List<SolutionItem> Children { get; } = new List<SolutionItem>();

        public SolutionItem(bool isFolder, string path)
        {
            IsFolder = isFolder;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            if (isFolder)
            {
                try
                {
                    foreach (var file in Directory.GetFiles(path))
                        Children.Add(new SolutionItem(false, file));
                }
                catch { }

                try
                {
                    foreach (var subDir in Directory.GetDirectories(path))
                        Children.Add(new SolutionItem(true, subDir));
                }
                catch { }
            }
        }
    }
}
