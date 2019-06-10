using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Libsdl.App;
using System.Reflection;
using Urho;
using Urho.Droid;

namespace BugWorkarounds.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Urho.Application app;
        private AbsoluteUrhoSurfacePlaceholder surface;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var layout = new FrameLayout(this);
            surface = new AbsoluteUrhoSurfacePlaceholder(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent)
            };
            layout.AddView(surface);
            SetContentView(layout);

            var options = new ApplicationOptions();
            app = await surface.Show<ScreenKeyboardBug>(options);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                Finish();
                return false;
            }
            if (!SDLActivity.DispatchKeyEvent(e))
            {
                return false;
            }
            return base.DispatchKeyEvent(e);
        }

        protected override void OnDestroy()
        {
            if (surface != null)
            {
                surface.Stop();
                var viewGroup = surface.Parent as ViewGroup;
                viewGroup?.RemoveView(surface);
            }
            base.OnDestroy();
        }

        public override void OnLowMemory()
        {
            SDLActivity.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnPause()
        {
            SDLActivity.OnPause();
            // Reflection <UrhoSurface>: Urho.Application.HandlePause();
            var handleResume = typeof(Urho.Application).GetMethod("HandlePause", BindingFlags.NonPublic | BindingFlags.Static);
            handleResume.Invoke(default, default);
            base.OnPause();
        }

        protected override void OnResume()
        {
            SDLActivity.OnResume();
            // Reflection <UrhoSurface>: Urho.Application.HandleResume();
            var handleResume = typeof(Urho.Application).GetMethod("HandleResume", BindingFlags.NonPublic | BindingFlags.Static);
            handleResume.Invoke(default, default);
            base.OnResume();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            SDLActivity.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    }
}
