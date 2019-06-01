using Urho;
using Urho.Gui;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class SdfFontsPreviewer : AbstractPreviewer
    {
        public SdfFontsPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            var text = node.CreateComponent<Text3D>();
            text.SetFont(ResourceCache.GetFont(asset.RelativePathToAsset), 24);
            text.SetColor(Color.Blue);
            text.Text = "The quick brown fox\njumps over the lazy dog";
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
