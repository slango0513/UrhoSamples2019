using Urho;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class UIPreviewer : AbstractPreviewer
    {
        public UIPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            App.UI.Root.SetDefaultStyle(ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));
            App.UI.LoadLayoutToElement(App.UI.Root, ResourceCache, asset.RelativePathToAsset);
        }

        protected override void OnStop()
        {
            base.OnStop();
            //App.UI.Root.RemoveAllChildren(); //NOTE: this will also remove MonoDebugHud (FPS counter)
        }
    }
}
