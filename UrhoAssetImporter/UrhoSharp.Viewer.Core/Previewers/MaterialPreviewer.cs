using System.Linq;
using Urho;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class MaterialPreviewer : AbstractPreviewer
    {
        StaticModel staticModel = null;
        int currentModelIndex = 0;

        public MaterialPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            var material = ResourceCache.GetMaterial(asset.RelativePathToAsset);

            var modelNode = node.CreateChild();
            modelNode.SetScale(2f);
            modelNode.Rotate(new Quaternion(30, 30, 30), TransformSpace.Local);
            staticModel = modelNode.CreateComponent<StaticModel>();

            //NOTE: this code went to Urho, will be replaced with the next Nuget update
            Technique[] result = new Technique[material.NumTechniques];
            for (uint i = 0; i < material.NumTechniques; i++)
                result[i] = material.GetTechnique(i);

            if (result.Any(t => t.Name.Contains(nameof(CoreAssets.Techniques.DiffSkybox))))
            {
                var skyNode = node.CreateChild();
                var skybox = skyNode.CreateComponent<Skybox>();
                skybox.Model = CoreAssets.Models.Box;
                skybox.SetMaterial(ResourceCache.GetMaterial(asset.RelativePathToAsset));
                staticModel.Model = CoreAssets.Models.Box;
            }
            else
            {
                staticModel.Model = CoreAssets.Models.Sphere;
            }
            staticModel.SetMaterial(material);
            App.Input.KeyDown += OnKeyDown;
        }

        protected override void OnStop()
        {
            App.Input.KeyDown -= OnKeyDown;
            base.OnStop();
        }

        void OnKeyDown(KeyDownEventArgs e)
        {
            if (e.Key == Key.F)
            {
                currentModelIndex++;
                switch (currentModelIndex)
                {
                    case 0:
                        staticModel.Model = CoreAssets.Models.Sphere;
                        break;
                    case 1:
                        staticModel.Model = CoreAssets.Models.Box;
                        break;
                    case 2:
                        staticModel.Model = CoreAssets.Models.Cylinder;
                        break;
                    case 3:
                        staticModel.Model = CoreAssets.Models.Cone;
                        currentModelIndex = -1;
                        break;
                }
            }
        }
    }
}
