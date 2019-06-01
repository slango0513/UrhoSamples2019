using System.IO;
using System.Linq;
using Urho;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class StaticModelPreviewer : AbstractPreviewer
    {
        public StaticModelPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            node.CreateComponent<Components.WirePlane>();
            StaticModel model = node.CreateComponent<StaticModel>();
            model.Model = ResourceCache.GetModel(asset.RelativePathToAsset);

            // try to find a material with the same name:
            var materials = FsUtils.FindFiles(asset.RootDirectory, f =>
            {
                var extension = Path.GetExtension(f).ToLowerInvariant();
                if (extension != ".xml") return false;
                var fileName = Path.GetFileNameWithoutExtension(f);
                if (fileName.Equals(Path.GetFileNameWithoutExtension(asset.AssetFileName),
                    System.StringComparison.InvariantCultureIgnoreCase))
                    return true;
                return false;
            })
                .OrderByDescending(f => f.ToLowerInvariant().Contains("material"))
                .ToArray();

            model.SetMaterial(materials.Any() ? ResourceCache.GetMaterial(materials.First()) : CreateDefaultMaterial());
            node.SetScaleBasedOnBoundingBox(60);
            var scaledTo = node.Scale.X;
            Editor?.DispatchToUI(() => Editor.DisplayModelScale(scaledTo));
        }
    }
}
