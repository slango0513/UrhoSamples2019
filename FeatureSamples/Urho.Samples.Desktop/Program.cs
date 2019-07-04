using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Urho.Samples.Desktop
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

        static Type[] samples;

        static void Main(string[] args)
        {
            var rootFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            LoadMonoUrho(rootFolder, true);

            FindAvailableSamplesAndPrint();
            Type selectedSampleType = null;

            if (args.Length > 0)
            {
                selectedSampleType = ParseSampleFromNumber(args[0]);
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                while (selectedSampleType == null)
                {
                    WriteLine("Enter a sample number:", ConsoleColor.White);
                    selectedSampleType = ParseSampleFromNumber(Console.ReadLine());
                }
            }
            else
            {
                selectedSampleType = typeof(Water);
            }

            var options = new ApplicationOptions()
            {
                ResourcePrefixPaths = new[] { rootFolder },
                ResourcePaths = new[] { "Data" },
                Width = 1280,
                Height = 720
            };
            if (Debugger.IsAttached && Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                options.NoSound = true;
                Debug.WriteLine("WARNING! Sound is disabled on Windows when debugger is attached (temporarily).");
            }
            var game = (Application)Activator.CreateInstance(selectedSampleType, options);
            var exitCode = game.Run();
            WriteLine($"Exit code: {exitCode}. Press any key to exit...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }

        static Type ParseSampleFromNumber(string input)
        {
            if (!int.TryParse(input, out int number))
            {
                WriteLine("Invalid format.", ConsoleColor.Red);
                return null;
            }

            if (number >= samples.Length || number < 0)
            {
                WriteLine("Invalid number.", ConsoleColor.Red);
                return null;
            }

            return samples[number];
        }

        static void FindAvailableSamplesAndPrint()
        {
            samples = typeof(Sample).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Application)) && t != typeof(Sample)).ToArray();
            int blockSize = samples.Length / 2;
            for (int i = 0; i < blockSize; i++)
                WriteLine(string.Format("{2,2}. {0,-30}{3,2}. {1,-30}", samples[i].Name, samples[i + blockSize].Name, i, i + blockSize), ConsoleColor.DarkGray);
            if (blockSize * 2 < samples.Length)
                WriteLine($"{samples.Length - 1,36}. {samples[samples.Length - 1].Name}", ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        static void WriteLine(string text, ConsoleColor consoleColor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
        }
    }
}
