using Urho;
using Urho.Urho2D;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class TmxPreviewer : AbstractPreviewer
    {
        public TmxPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        public override bool RotateRootNode => false;

        protected override void OnShow(Node node, Asset asset)
        {
            TileMap2D tileMap = node.CreateComponent<TileMap2D>();
            TmxFile2D tmxFile = ResourceCache.GetTmxFile2D(asset.RelativePathToAsset);
            tileMap.TmxFile = tmxFile;
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}
