using Urho;
using Urho.Actions;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class Particle3DPreviewer : AbstractPreviewer
    {
        public Particle3DPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            node.SetScale(1f);
            ParticleEffect particleEffect = ResourceCache.GetParticleEffect(asset.RelativePathToAsset);
            if (particleEffect == null)
                return;

            ParticleEmitter particleEmitter = node.CreateComponent<ParticleEmitter>();
            particleEmitter.Effect = particleEffect;
            App.Viewport.SetClearColor(Color.Black);

            //TODO: remove it and let user to control position by mouse
            node.RunActions(new RepeatForever(new RotateAroundBy(0.5f, new Vector3(1, 0, 0), 0, 0, 90, TransformSpace.Parent)));
        }

        protected override void OnStop()
        {
            App.ResetClearColor();
        }
    }
}
