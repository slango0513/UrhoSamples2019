using System;
using System.IO;
using System.Runtime.InteropServices;
using Urho;

namespace SamplyGame.Desktop
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        static void LoadMonoUrho(string rootFolder, bool isD3D)
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return; //on macOS/Linux the libs are fat and there is no DirectX
            }
            var relativePathToLib = Path.Combine($@"Win{(IntPtr.Size == 8 ? "64" : "32")}_{(isD3D ? "DirectX" : "OpenGL")}", $"mono-urho.dll");
            Console.WriteLine($"Loaded library: {relativePathToLib}");
            var file = Path.Combine(rootFolder, relativePathToLib);
            _ = LoadLibrary(file);
        }

        static void Main(string[] args)
        {
            var rootFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            LoadMonoUrho(rootFolder, true);

            var options = new ApplicationOptions()
            {
                ResourcePrefixPaths = new[] { rootFolder },
                ResourcePaths = new[] { "Data" },
                Width = 720,
                Height = 1280
            };
            new SamplyGame(options).Run();
        }
    }
}
