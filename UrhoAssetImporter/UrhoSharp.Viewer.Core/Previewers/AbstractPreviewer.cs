using Urho;
using Urho.Resources;

namespace UrhoSharp.Viewer.Core.Previewers
{
    public abstract class AbstractPreviewer
    {
        protected UrhoScene App { get; }
        protected ResourceCache ResourceCache => App.ResourceCache;
        protected Node Node { get; private set; }
        protected Scene Scene => Node?.Scene;
        protected Asset Asset { get; private set; }
        protected IEditor Editor { get; private set; }

        protected AbstractPreviewer(UrhoScene urhoApp)
        {
            App = urhoApp;
        }

        protected abstract void OnShow(Node node, Asset asset);

        protected virtual void OnStop() { }

        public virtual bool RotateRootNode => true;

        public void Show(Node node, Asset asset, IEditor editor)
        {
            Node = node;
            Asset = asset;
            Editor = editor;
            Editor?.DispatchToUI(() => Editor.DisplayModelScale(0f));
            OnShow(node, asset);
            App.Update += OnUpdate;
        }

        public void Stop()
        {
            App.Update -= OnUpdate;
            OnStop();
        }

        protected Material CreateDefaultMaterial()
        {
            var material = ResourceCache.GetMaterial("Materials/DefaultGrey.xml").Clone(string.Empty);
            //material.SetShaderParameter("MatSpecColor", Color.Magenta);
            return material;
        }

        protected virtual void OnUpdate(UpdateEventArgs e) { }
    }
}
