using System;
using Urho.Gui;

namespace Urho.Samples
{
    public class RibbonTrailDemo : Sample
    {
        protected RibbonTrail swordTrail;
        protected AnimationController ninjaAnimCtrl;
        protected float swordTrailStartTime;
        protected float swordTrailEndTime;
        protected Node boxNode1;
        protected Node boxNode2;
        protected float timeStepSum;

        private Scene scene;

        public RibbonTrailDemo(ApplicationOptions options = null) : base(options)
        {
            swordTrailStartTime = 0.2f;
            swordTrailEndTime = 0.46f;
            timeStepSum = 0.0f;
        }

        protected override void Start()
        {
            // Execute base class startup
            base.Start();

            // Create the scene content
            CreateScene();

            // Create the UI content
            SimpleCreateInstructionsWithWasd();

            // Setup the viewport for displaying the scene
            SetupViewport();
        }

        private void CreateScene()
        {
            var cache = ResourceCache;

            scene = new Scene(Context);

            // Create octree, use default volume (-1000, -1000, -1000) to (1000, 1000, 1000)
            scene.CreateComponent<Octree>();

            // Create scene node & StaticModel component for showing a static plane
            var planeNode = scene.CreateChild("Plane");
            planeNode.Scale = new Vector3(100.0f, 1.0f, 100.0f);
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = cache.GetModel("Models/Plane.mdl");
            planeObject.SetMaterial(cache.GetMaterial("Materials/StoneTiled.xml"));

            // Create a directional light to the world.
            var lightNode = scene.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f)); // The direction vector does not need to be normalized
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.CastShadows = true;
            light.ShadowBias = new BiasParameters(0.00005f, 0.5f);
            // Set cascade splits at 10, 50 and 200 world units, fade shadows out at 80% of maximum shadow distance
            light.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);

            // Create first box for face camera trail demo with 1 column.
            boxNode1 = scene.CreateChild("Box1");
            var box1 = boxNode1.CreateComponent<StaticModel>();
            box1.Model = cache.GetModel("Models/Box.mdl");
            box1.CastShadows = true;
            var boxTrail1 = boxNode1.CreateComponent<RibbonTrail>();
            boxTrail1.Material = cache.GetMaterial("Materials/RibbonTrail.xml");
            boxTrail1.StartColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            boxTrail1.EndColor = new Color(1.0f, 1.0f, 0.0f, 0.0f);
            boxTrail1.Width = 0.5f;
            boxTrail1.UpdateInvisible = true;

            // Create second box for face camera trail demo with 4 column.
            // This will produce less distortion than first trail.
            boxNode2 = scene.CreateChild("Box2");
            var box2 = boxNode2.CreateComponent<StaticModel>();
            box2.Model = cache.GetModel("Models/Box.mdl");
            box2.CastShadows = true;
            var boxTrail2 = boxNode2.CreateComponent<RibbonTrail>();
            boxTrail2.Material = cache.GetMaterial("Materials/RibbonTrail.xml");
            boxTrail2.StartColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            boxTrail2.EndColor = new Color(1.0f, 1.0f, 0.0f, 0.0f);
            boxTrail2.Width = 0.5f;
            boxTrail2.TailColumn = 4;
            boxTrail2.UpdateInvisible = true;

            // Load ninja animated model for bone trail demo.
            var ninjaNode = scene.CreateChild("Ninja");
            ninjaNode.Position = new Vector3(5.0f, 0.0f, 0.0f);
            ninjaNode.Rotation = new Quaternion(0.0f, 180.0f, 0.0f);
            var ninja = ninjaNode.CreateComponent<AnimatedModel>();
            ninja.SetModel(cache.GetModel("Models/NinjaSnowWar/Ninja.mdl"));
            ninja.SetMaterial(cache.GetMaterial("Materials/NinjaSnowWar/Ninja.xml"));
            ninja.CastShadows = true;

            // Create animation controller and play attack animation.
            ninjaAnimCtrl = ninjaNode.CreateComponent<AnimationController>();
            ninjaAnimCtrl.PlayExclusive("Models/NinjaSnowWar/Ninja_Attack3.ani", 0, true, 0.0f);

            // Add ribbon trail to tip of sword.
            var swordTip = ninjaNode.GetChild("Joint29", true);
            swordTrail = swordTip.CreateComponent<RibbonTrail>();

            // Set sword trail type to bone and set other parameters.
            swordTrail.TrailType = TrailType.Bone;
            swordTrail.Material = cache.GetMaterial("Materials/SlashTrail.xml");
            swordTrail.Lifetime = 0.22f;
            swordTrail.StartColor = new Color(1.0f, 1.0f, 1.0f, 0.75f);
            swordTrail.EndColor = new Color(0.2f, 0.5f, 1.0f, 0.0f);
            swordTrail.TailColumn = 4;
            swordTrail.UpdateInvisible = true;

            // Add floating text for info.
            var boxTextNode1 = scene.CreateChild("BoxText1");
            boxTextNode1.Position = new Vector3(-1.0f, 2.0f, 0.0f);
            var boxText1 = boxTextNode1.CreateComponent<Text3D>();
            boxText1.Text = "Face Camera Trail (4 Column)";
            boxText1.SetFont(cache.GetFont("Fonts/BlueHighway.sdf"), 24);

            var boxTextNode2 = scene.CreateChild("BoxText2");
            boxTextNode2.Position = new Vector3(-6.0f, 2.0f, 0.0f);
            var boxText2 = boxTextNode2.CreateComponent<Text3D>();
            boxText2.Text = "Face Camera Trail (1 Column)";
            boxText2.SetFont(cache.GetFont("Fonts/BlueHighway.sdf"), 24);

            var ninjaTextNode2 = scene.CreateChild("NinjaText");
            ninjaTextNode2.Position = new Vector3(4.0f, 2.5f, 0.0f);
            var ninjaText = ninjaTextNode2.CreateComponent<Text3D>();
            ninjaText.Text = "Bone Trail (4 Column)";
            ninjaText.SetFont(cache.GetFont("Fonts/BlueHighway.sdf"), 24);

            // Create the camera.
            CameraNode = scene.CreateChild("Camera");
            CameraNode.CreateComponent<Camera>();

            // Set an initial position for the camera scene node above the plane
            CameraNode.Position = new Vector3(0.0f, 2.0f, -14.0f);
        }

        private void SetupViewport()
        {
            var renderer = Renderer;

            // Set up a viewport to the Renderer subsystem so that the 3D scene can be seen. We need to define the scene and the camera
            // at minimum. Additionally we could configure the viewport screen size and the rendering path (eg. forward / deferred) to
            // use, but now we just use full screen and default render path configured in the engine command line options
            var viewport = new Viewport(Context, scene, CameraNode.GetComponent<Camera>());
            renderer.SetViewport(0, viewport);
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            SimpleMoveCamera3D(timeStep);

            // Sum of timesteps.
            timeStepSum += timeStep;

            // Move first box with pattern.
            boxNode1.SetTransform(new Vector3(-4.0f + 3.0f * (float)Math.Cos(100.0f * timeStepSum * MathHelper.DTORF), 0.5f, -2.0f * (float)Math.Cos(400.0f * timeStepSum * MathHelper.DTORF)), new Quaternion());

            // Move second box with pattern.
            boxNode2.SetTransform(new Vector3(0.0f + 3.0f * (float)Math.Cos(100.0f * timeStepSum * MathHelper.DTORF), 0.5f, -2.0f * (float)Math.Cos(400.0f * timeStepSum * MathHelper.DTORF)), new Quaternion());

            // Get elapsed attack animation time.
            var swordAnimTime = ninjaAnimCtrl.GetAnimationState("Models/NinjaSnowWar/Ninja_Attack3.ani").Time;

            // Stop emitting trail when sword is finished slashing.
            if (!swordTrail.Emitting && swordAnimTime > swordTrailStartTime && swordAnimTime < swordTrailEndTime)
                swordTrail.Emitting = true;
            else if (swordTrail.Emitting && swordAnimTime >= swordTrailEndTime)
                swordTrail.Emitting = false;
        }
    }
}
