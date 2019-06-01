using Urho;

namespace UrhoSharp.Viewer.Core.Components
{
    public class Gizmo : Component
    {
        Node nodeToSync;

        public Gizmo()
        {
            ReceiveSceneUpdates = true;
        }

        public override void OnAttachedToNode(Node node)
        {
            CustomGeometry geom = node.CreateComponent<CustomGeometry>();
            geom.BeginGeometry(0, PrimitiveType.LineList);
            var material = new Material();
            material.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);
            geom.SetMaterial(material);


            float size = 1;

            //x
            geom.DefineVertex(Vector3.Zero);
            geom.DefineColor(Color.Red);
            geom.DefineVertex(Vector3.UnitX * size);
            geom.DefineColor(Color.Red);
            //y
            geom.DefineVertex(Vector3.Zero);
            geom.DefineColor(Color.Green);
            geom.DefineVertex(Vector3.UnitY * size);
            geom.DefineColor(Color.Green);
            //z
            geom.DefineVertex(Vector3.Zero);
            geom.DefineColor(Color.Blue);
            geom.DefineVertex(Vector3.UnitZ * size);
            geom.DefineColor(Color.Blue);

            geom.Commit();

            base.OnAttachedToNode(node);
        }

        protected override void OnUpdate(float timeStep)
        {
            if (nodeToSync != null)
                Node.SetWorldRotation(nodeToSync.WorldRotation);
            base.OnUpdate(timeStep);
        }

        public void SyncronizeRotationWith(Node node)
        {
            nodeToSync = node;
        }
    }
}
