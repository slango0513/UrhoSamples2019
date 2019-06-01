using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Urho;

namespace UrhoSharp.Viewer.Core
{
    /// <summary>
    /// </summary>
    public class PreviewerApplication
    {
        UrhoScene urhoScene;
        Asset currentAsset;
        IEditor editor;
        readonly AssetsResolver assetsResolver;
        static string lastLoadedFile;
        static CancellationTokenSource cancellationSource = new CancellationTokenSource();
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public PreviewerApplication(AssetsResolver assetsResolver)
        {
            this.assetsResolver = assetsResolver;
        }

        static PreviewerApplication()
        {
            Application.UnhandledException += (s, e) =>
                {
                    e.Handled = true;
                };
        }

        public async Task<UrhoScene> Show(string file, IEditor editor, int initialWidth = 1000, int initialHeight = 1000)
        {
            try
            {
                this.editor = editor;
                lastLoadedFile = file;
                cancellationSource.Cancel();
                cancellationSource = new CancellationTokenSource();
                await semaphoreSlim.WaitAsync();
                var token = cancellationSource.Token;
                token.Register(Close);

                var asset = await assetsResolver.ResolveAsset(file, token);
                if (asset == null) throw new NotSupportedException($"{file} is not supported");
                token.ThrowIfCancellationRequested();

                token.ThrowIfCancellationRequested();
                Debug.WriteLine($"PreviewerApplication.Show for: {file}, resolved asset={asset}, asm.location={Assembly.GetExecutingAssembly().Location}");

                if (Application.HasCurrent)
                    await Application.Current.Exit();
                token.ThrowIfCancellationRequested();

                IsPaused = false;

                IntPtr surfacePtr = SurfaceRecreationRequested?.Invoke() ?? IntPtr.Zero;
                if (surfacePtr == IntPtr.Zero)
                    return null;

                urhoScene = new UrhoScene(new ApplicationOptions
                {
                    ExternalWindow = surfacePtr,
                    ResourcePaths = new[] { asset.RootDirectory, /*"ViewerAssets"*/ },

                    Width = initialWidth,
                    Height = initialHeight,
                    //AutoloadCoreData = false,
                    LimitFps = false,
                    ResizableWindow = true,
                    Multisampling = Environment.OSVersion.Platform == PlatformID.Win32NT ? 0 : 8,
                    ResourcePrefixPaths = new[] { Environment.CurrentDirectory },
                    DelayedStart = true,
                });

                token.ThrowIfCancellationRequested();
                urhoScene.Run();
                token.ThrowIfCancellationRequested();

                currentAsset = asset;
                urhoScene.ShowAsset(asset, editor);
                RunFrames();
                return urhoScene;
            }
            catch (Exception)
            {
                Close();
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        async void RunFrames()
        {
            if (!urhoScene.Options.DelayedStart)
                return;
            var handle = urhoScene.Handle;

            while (urhoScene != null &&
                !urhoScene.IsExiting &&
                !IsPaused &&
                handle == urhoScene.Handle)
            {
                urhoScene.Engine.RunFrame();
                await Task.Delay(TimeSpan.FromMilliseconds(1000f / FpsLimit));
            }
        }

        public event Func<IntPtr> SurfaceRecreationRequested;

        public void Close()
        {
            Debug.WriteLine($"PreviewerApplication.Close for asset={currentAsset}");
            urhoScene?.Exit();
            urhoScene = null;
        }

        public bool IsPaused { get; private set; }
        public const int ActiveTabFpsLimit = 50;
        public const int InactiveTabFpsLimit = 24;
        public int FpsLimit { get; private set; } = ActiveTabFpsLimit;

        public async void OnActivated()
        {
            if (currentAsset != null && lastLoadedFile != currentAsset.OriginalFile)
            {
                FpsLimit = ActiveTabFpsLimit;
                try
                {
                    await Show(currentAsset.OriginalFile, editor);
                }
                catch (Exception exc) { Debug.WriteLine(exc); }
            }
            else if (currentAsset != null && Application.HasCurrent)
            {
                FpsLimit = ActiveTabFpsLimit;
                if (IsPaused)
                {
                    IsPaused = false;
                    RunFrames();
                }
            }
        }

        public void OnDeactivated()
        {
            if (currentAsset != null && lastLoadedFile == currentAsset.OriginalFile)
            {
                FpsLimit = InactiveTabFpsLimit;
                //IsPaused = true;
            }
        }
    }
}
