using Urho;
using Urho.Actions;
using Urho.Urho2D;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class Particle2DPreviewer : AbstractPreviewer
    {
        public Particle2DPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        protected override void OnShow(Node node, Asset asset)
        {
            node.SetScale(1f);
            ParticleEffect2D particleEffect = ResourceCache.GetParticleEffect2D(asset.RelativePathToAsset);
            if (particleEffect == null)
                return;

            ParticleEmitter2D particleEmitter = node.CreateComponent<ParticleEmitter2D>();
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
