using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Urho;
using Urho.Droid;

namespace SamplyGame.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var layout = new FrameLayout(this);
            var surface = UrhoSurface.CreateSurface(this);
            layout.AddView(surface);
            SetContentView(layout);

            var options = new ApplicationOptions()
            {
                ResourcePaths = new[] { "Data" }
            };
            var app = await surface.Show<SamplyGame>(options);
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
            if (!UrhoSurface.DispatchKeyEvent(e))
            {
                return false;
            }
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    }
}
