using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Urho.Droid;

namespace Urho.Samples.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        private Application app;
        private UrhoSurfacePlaceholder surface;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            var layout = new FrameLayout(this);
            surface = UrhoSurface.CreateSurface(this);
            layout.AddView(surface);
            SetContentView(layout);

            var type = Type.GetType(Intent.GetStringExtra("Type"));
            var options = new ApplicationOptions()
            {
                ResourcePaths = new[] { "Data" }
            };
            app = await surface.Show(type, options);
        }

        protected override void OnResume()
        {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            UrhoSurface.OnDestroy();
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                Finish();
                return false;
            }
            //if (!UrhoSurface.DispatchKeyEvent(e))
            //{
            //    return false;
            //}
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    }
}
