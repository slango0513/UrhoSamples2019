using Urho;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class RenderPathPreviewer : AbstractPreviewer
    {
        RenderPath oldRp;

        public RenderPathPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            oldRp = App.Viewport.RenderPath.Clone();
            RenderPath customRp = oldRp.Clone();
            customRp.Append(ResourceCache.GetXmlFile(asset.RelativePathToAsset));
            App.Viewport.RenderPath = customRp;

            var model = node.CreateComponent<StaticModel>();
            model.Model = ResourceCache.GetModel("Models/TeaPot.mdl");
            model.SetMaterial(ResourceCache.GetMaterial("Materials/DefaulPreviewerMaterial.xml"));

            node.SetScale(5f);
            node.Rotation = new Quaternion(20, 150, 20);
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.Viewport.RenderPath = oldRp;
        }
    }
}
