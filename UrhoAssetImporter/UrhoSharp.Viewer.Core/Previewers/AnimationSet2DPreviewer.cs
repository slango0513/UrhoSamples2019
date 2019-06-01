using Urho;
using Urho.Urho2D;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public class AnimationSet2DPreviewer : AbstractPreviewer
    {
        public AnimationSet2DPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

        public override bool RotateRootNode => false;

        protected override void OnShow(Node node, Asset asset)
        {
            AnimationSet2D animationSet = ResourceCache.GetAnimationSet2D(asset.RelativePathToAsset);
            AnimatedSprite2D animatedSprite = node.CreateComponent<AnimatedSprite2D>();
            animatedSprite.AnimationSet = animationSet;
            animatedSprite.SetAnimation(animationSet.GetAnimation(0), LoopMode2D.Default);
        }
    }
}
