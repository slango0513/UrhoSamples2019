using Urho;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class TexturePreviewer : AbstractPreviewer
    {
        public TexturePreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            var modelNode = node.CreateChild();
            modelNode.SetScale(2f);
            modelNode.Rotate(new Quaternion(30, 30, 30), TransformSpace.Local);
            var staticModel = modelNode.CreateComponent<StaticModel>();
            staticModel.Model = CoreAssets.Models.Sphere;
            staticModel.SetMaterial(Material.FromImage(asset.RelativePathToAsset));
        }
    }
}
