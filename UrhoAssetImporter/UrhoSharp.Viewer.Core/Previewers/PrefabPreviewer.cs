using System;
using Urho;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class PrefabPreviewer : AbstractPreviewer
    {
        Node prefabNode;
        float scale;
        Material selectedMaterial;
        StaticModel selectedModel;
        StaticModel prevSourceStaticModel;
        StaticModel selectedStaticModel;

        public PrefabPreviewer(UrhoScene urhoApp) : base(urhoApp)
        {
        }

        protected override void OnShow(Node node, Asset asset)
        {
            App.Input.MouseButtonUp += Input_MouseButtonUp;
            App.Input.KeyUp += Input_KeyUp;
            node.CreateComponent<Components.WirePlane>();
            Refresh();
        }

        void Input_KeyUp(KeyUpEventArgs e)
        {
            switch (e.Key)
            {
                case Key.X:
                    RotateAxis(e.Qualifiers > 0 ? -90 : 90, 0, 0);
                    break;
                case Key.Y:
                    RotateAxis(0, e.Qualifiers > 0 ? -90 : 90, 0);
                    break;
                case Key.Z:
                    RotateAxis(0, 0, e.Qualifiers > 0 ? -90 : 90);
                    break;
            }
        }

        void RotateAxis(float x, float y, float z)
        {
            prefabNode.Rotate(new Quaternion(x, y, z), TransformSpace.Local);
        }

        protected override void OnStop()
        {
            App.Input.MouseButtonUp -= Input_MouseButtonUp;
            App.Input.MouseButtonUp += e =>
            {
                var cursorPos = App.UI.CursorPosition;
                var cameraRay = App.Camera.GetScreenRay((float)cursorPos.X / App.Graphics.Width, (float)cursorPos.Y / App.Graphics.Height);
                var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 10000, DrawableFlags.Geometry);
                if (result != null)
                {
                    var model = (StaticModel)result.Value.Drawable;
                    if (model != null)
                    {
                        var material = model.GetMaterial(0);
                        if (material != null)
                            material.FillMode = material.FillMode == FillMode.Wireframe ? FillMode.Solid : FillMode.Wireframe;
                    }
                    model.SetMaterial(Material.FromColor(Color.Red));
                }
            };

            App.Input.KeyUp -= Input_KeyUp;
            base.OnStop();
        }

        Material CreateSelectionMaterial()
        {
            var mat = Material.FromColor(Color.Blue);
            mat.FillMode = FillMode.Wireframe;
            var specColorAnimation = new ValueAnimation();

            Color color = new Color(0.8f, 0.8f, 0.1f);
            Color fade = new Color(0.5f, 0.5f, 0.5f);

            specColorAnimation.SetKeyFrame(0.0f, fade);
            specColorAnimation.SetKeyFrame(0.5f, color);
            specColorAnimation.SetKeyFrame(1.0f, fade);
            mat.SetShaderParameterAnimation("MatDiffColor", specColorAnimation, WrapMode.Loop, 1.0f);
            //mat.AddRef();
            return mat;
        }

        void Input_MouseButtonUp(MouseButtonUpEventArgs e)
        {
            if (prevSourceStaticModel != null && !prevSourceStaticModel.IsDeleted)
            {
                prevSourceStaticModel.Enabled = true;
                if (selectedStaticModel != null && !selectedStaticModel.IsDeleted)
                    selectedStaticModel.Remove();
            }

            var cursorPos = App.UI.CursorPosition;
            var cameraRay = App.Camera.GetScreenRay((float)cursorPos.X / App.Graphics.Width, (float)cursorPos.Y / App.Graphics.Height);
            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 10000, DrawableFlags.Geometry);

            if (result != null)
            {
                var geometry = result.Value.Drawable as StaticModel;
                if (geometry != null)
                {
                    prevSourceStaticModel = geometry;
                    geometry.Enabled = false;
                    selectedStaticModel = geometry.Node.CreateComponent<StaticModel>();
                    selectedStaticModel.Model = geometry.Model;
                    selectedStaticModel.SetMaterial(CreateSelectionMaterial());

                    var nodeName = result.Value.Node.Name;
                    Editor?.DispatchToUI(() => Editor.HighlightXmlForNode(nodeName));
                }
            }
        }

        public void Refresh()
        {
            var file = ResourceCache.GetFile(Asset.RelativePathToAsset, true);
            if (file == null)
                throw new InvalidOperationException($"{Asset} not found");

            prefabNode?.Remove();
            prefabNode = Scene.InstantiateXml(file, Vector3.Zero, Quaternion.Identity, CreateMode.Replicated);
            if (prefabNode != null)
            {
                prefabNode.ChangeParent(Node);
                //prefabNode.Translate(Vector3.Up * 0.75f);
            }

            if (scale == 0 && prefabNode != null)
            {
                prefabNode?.SetScaleBasedOnBoundingBox(10);
                scale = prefabNode.Scale.X;
            }
            else
            {
                prefabNode?.SetScale(scale);
            }
            Editor?.DispatchToUI(() => Editor.DisplayModelScale(scale));
        }

    }
}
