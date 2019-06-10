using Android.App;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Org.Libsdl.App;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Urho.Droid
{
    public class AbsoluteUrhoSurfacePlaceholder : AbsoluteLayout
    {
        private bool launching;
        public SDLSurface SDLSurface;

        public AbsoluteUrhoSurfacePlaceholder(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public AbsoluteUrhoSurfacePlaceholder(Android.Content.Context context) : base(context)
        {
        }

        public AbsoluteUrhoSurfacePlaceholder(Android.Content.Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public AbsoluteUrhoSurfacePlaceholder(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        public AbsoluteUrhoSurfacePlaceholder(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public async Task<Application> Show(Type appType, ApplicationOptions options = null, bool finishActivityOnExit = false)
        {
            Stop();
            launching = true;
            if (SDLSurface != null)
            {
                RemoveView(SDLSurface);
            }

            SDLSurface = SDLActivity.CreateSurface(Context as Activity);
            AddView(SDLSurface, ViewGroup.LayoutParams.MatchParent);
            // Reflection <UrhoSurfacePlaceholder>: Urho.Application.CurrentSurface = new WeakReference(SDLSurface);
            var currentSurface = typeof(Application).GetProperty(nameof(Application.CurrentSurface), BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
            currentSurface.SetValue(default, new WeakReference(SDLSurface));
            // Reflection <UrhoSurfacePlaceholder>: Urho.Application.CurrentWindow = new WeakReference(Context as Activity);
            var currentWindow = typeof(Application).GetProperty(nameof(Application.CurrentWindow), BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
            currentWindow.SetValue(default, new WeakReference(Context as Activity));

            var tcs = new TaskCompletionSource<Application>();
            void startedHandler()
            {
                Application.Started -= startedHandler;
                tcs.TrySetResult(Application.Current);
            }

            Application.Started += startedHandler;
            //UrhoSurface.SetSdlMain(() => Urho.Application.CreateInstance(appType, options), finishActivityOnExit, SDLSurface);
            var setSdlMain = typeof(UrhoSurface).GetMethod("SetSdlMain", BindingFlags.NonPublic | BindingFlags.Static);
            setSdlMain.Invoke(default, new object[] { new Func<Application>(() => Application.CreateInstance(appType, options)), finishActivityOnExit, SDLSurface });
            var app = await tcs.Task;
            launching = false;
            return app;
        }

        public async Task<TApplication> Show<TApplication>(ApplicationOptions options = null, bool finishActivityOnExit = false) where TApplication : Application
        {
            var app = await Show(typeof(TApplication), options, finishActivityOnExit);
            return (TApplication)app;
        }

        public void Stop()
        {
            if (launching)
            {
                // Reflection <UrhoSurfacePlaceholder>: Urho.Application.WaitStart();
                var waitStart = typeof(Application).GetMethod("WaitStart", BindingFlags.NonPublic | BindingFlags.Static);
                waitStart.Invoke(default, default);
                Console.WriteLine("WARNING: Stop while starting the urho app.");
            }
            //TODO: make sure Stop() is called in the main Android thread (not in the game thread)
            // Reflection <UrhoSurfacePlaceholder>: Urho.Application.StopCurrent();
            var stopCurrent = typeof(Application).GetMethod("StopCurrent", BindingFlags.NonPublic | BindingFlags.Static);
            stopCurrent.Invoke(default, default);
        }
    }
}
