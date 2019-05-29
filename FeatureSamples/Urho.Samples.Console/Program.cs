using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Urho.Samples.Mac
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        static Type[] samples;

        /// <param name="args">sample number, e.g. "19"</param>
        static void Main(string[] args)
        {
            samples = typeof(Sample).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Application)) && t != typeof(Sample)).ToArray();
            Type selectedSampleType = args.Length > 0 ? ParseSampleFromNumber(args[0]) : typeof(BasicTechniques);

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
            Console.WriteLine($"Exit code: {exitCode}. Press any key to exit...");
            Console.ReadKey();
        }

        static Type ParseSampleFromNumber(string input)
        {
            if (!int.TryParse(input, out int number))
            {
                Console.WriteLine("Invalid format.");
                return null;
            }

            if (number >= samples.Length || number < 0)
            {
                Console.WriteLine("Invalid number.");
                return null;
            }

            return samples[number];
        }
    }
}
