using System;
using System.IO;
using System.Linq;
using Urho;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class AnimatedModelPreviewer : AbstractPreviewer
    {
        AnimatedModel model;

        public AnimatedModelPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            node.CreateComponent<Components.WirePlane>();
            //Here we should find a model to apply ANI
            //TODO: ask user if not sure
            var modelName = Directory.GetFiles(Path.GetDirectoryName(asset.FullPathToAsset), "*.mdl")
                .Select(Path.GetFileNameWithoutExtension)
                .FirstOrDefault(f => asset.AssetFileName.StartsWith(f));

            if (string.IsNullOrEmpty(modelName))
                throw new Exception("Can't find a model to apply selected animation.");

            model = node.CreateComponent<AnimatedModel>();
            model.Model = ResourceCache.GetModel(Path.Combine(Path.GetDirectoryName(asset.RelativePathToAsset), modelName + ".mdl"));
            model.SetMaterial(CreateDefaultMaterial());

            var walkAnimation = ResourceCache.GetAnimation(asset.RelativePathToAsset);
            var state = model.AddAnimationState(walkAnimation);
            if (state != null)
            {
                state.Weight = 1;
                state.Looped = true;
            }
            node.SetScaleBasedOnBoundingBox(60);
            var scaledTo = node.Scale.X;
            Editor?.DispatchToUI(() => Editor.DisplayModelScale(scaledTo));
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if (model.NumAnimationStates > 0)
                model?.AnimationStates?.FirstOrDefault()?.AddTime(e.TimeStep);
        }
    }
}
