using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Urho.Samples.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        void LoadMonoUrho(string rootFolder, bool isD3D)
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return; //on macOS/Linux the libs are fat and there is no DirectX
            }
            Debug.WriteLine($"RootFolder: {rootFolder}");
            var relativePathToLib = Path.Combine($@"Win{(IntPtr.Size == 8 ? "64" : "32")}_{(isD3D ? "DirectX" : "OpenGL")}", $"mono-urho.dll");
            Debug.WriteLine($"Loaded library: {relativePathToLib}");
            var file = Path.Combine(rootFolder, relativePathToLib);
            _ = LoadLibrary(file);
        }

        string rootFolder;

        TypeInfo selectedGameType;

        public MainWindow()
        {
            rootFolder = Path.GetDirectoryName(typeof(App).Assembly.Location);
            LoadMonoUrho(rootFolder, true);

            InitializeComponent();

            //DesktopUrhoInitializer.AssetsDirectory = @"../../Assets";
            GameTypes = typeof(Sample).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Application)) && t != typeof(Sample))
                .Select((t, i) => new TypeInfo(t, $"{i + 1}. {t.Name}", ""))
                .ToArray();
            DataContext = this;
            Loaded += (s, e) => SelectedGameType = GameTypes[19]; //water
        }

        public TypeInfo[] GameTypes { get; set; }

        public TypeInfo SelectedGameType
        {
            get => selectedGameType;
            set
            {
                RunGame(value);
                selectedGameType = value;
            }
        }

        async void RunGame(TypeInfo value)
        {
            var options = new ApplicationOptions()
            {
                ResourcePrefixPaths = new[] { rootFolder },
                ResourcePaths = new[] { "Data" }
            };
            if (Debugger.IsAttached && Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                options.NoSound = true;
                Debug.WriteLine("WARNING! Sound is disabled on Windows when debugger is attached (temporarily).");
            }
            var app = await UrhoSurfaceCtrl.Show(value.Type, options);
            Application.InvokeOnMain(() => { /*app.DoSomeStuff();*/});
        }
    }

    public class TypeInfo
    {
        public Type Type { get; }
        public string Name { get; }
        public string Description { get; }

        public TypeInfo(Type type, string name, string description)
        {
            Type = type;
            Name = name;
            Description = description;
        }
    }
}
