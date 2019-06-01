using Urho;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class ScenePreviewer : AbstractPreviewer
    {
        public ScenePreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override async void OnShow(Node node, Asset asset)
        {
            App.Scene.Remove();
            App.CreateScene(asset.RelativePathToAsset);
        }

        protected override void OnStop()
        {
            App.Scene.Remove();
            App.CreateScene(null);
        }
    }
}
