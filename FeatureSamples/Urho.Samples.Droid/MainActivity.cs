using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;

namespace Urho.Samples.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : ListActivity
    {
        Type[] sampleTypes;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            //Show a list of available samples (click to run):
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.samples_list_text_view);
            sampleTypes = typeof(Sample).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Application))
                && t != typeof(Sample) /*&& t != typeof(PBRMaterials) && t != typeof(BasicTechniques)*/).ToArray();
            foreach (var sample in sampleTypes)
            {
                adapter.Add(sample.Name);
            }
            SetContentView(Resource.Layout.activity_main);
            ListAdapter = adapter;
        }

        protected override void OnListItemClick(ListView l, Android.Views.View v, int position, long id)
        {
            Console.WriteLine($"OnListItemClick {l} {v} {position} {id}");
            var intent = new Intent(this, typeof(GameActivity));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
            intent.PutExtra("Type", sampleTypes[position].AssemblyQualifiedName);
            StartActivity(intent);
        }
    }
}
