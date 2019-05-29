using System;
using Urho;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UrhoPage : ContentPage
    {
        private Charts urhoApp;

        public UrhoPage()
        {
            InitializeComponent();
        }

        private void RestartButton_Clicked(object sender, EventArgs e)
        {
            StartUrhoApp();
        }

        private void RotationSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (urhoApp == default)
            {
                return;
            }
            urhoApp.Rotate((float)(e.NewValue - e.OldValue));
        }

        private void SelectedBarSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (urhoApp == default)
            {
                return;
            }
            if (urhoApp.SelectedBar == default)
            {
                return;
            }
            urhoApp.SelectedBar.Value = (float)e.NewValue;
        }

        protected override void OnAppearing()
        {
            StartUrhoApp();
        }

        async void StartUrhoApp()
        {
            urhoApp = await UrhoSurface.Show<Charts>(new ApplicationOptions(assetsFolder: default) { Orientation = ApplicationOptions.OrientationType.LandscapeAndPortrait });
        }
    }
}
