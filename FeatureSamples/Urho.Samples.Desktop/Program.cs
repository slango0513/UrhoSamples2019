using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Urho.Samples.Desktop
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        static Type[] samples;

        static void Main(string[] args)
        {
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

            var assmLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var monoUrhoPath = Path.Combine(assmLocation, "Win64_OpenGL", "mono-urho.dll");
            _ = LoadLibrary(monoUrhoPath);

            var options = new ApplicationOptions()
            {
                ResourcePrefixPaths = new[] { assmLocation },
                ResourcePaths = new[] { "Data" }
            };
            var game = (Application)Activator.CreateInstance(selectedSampleType, options);
            var exitCode = game.Run();
            WriteLine($"Exit code: {exitCode}. Press any key to exit...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }

        static Type ParseSampleFromNumber(string input)
        {
            int number;
            if (!int.TryParse(input, out number))
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
