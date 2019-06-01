using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.IO;
using Urho.Physics;
using Urho.Shapes;
using UrhoSharp.Viewer.Core.Previewers;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core
{
    public class UrhoScene : Application
    {
        Asset currentAsset;
        AbstractPreviewer currentPreviewer;
        Node rootNode;
        Node lightNode;
        Light light;
        bool rotateActionStarted;
        Text errorText;
        bool defaultScene;
        UrhoConsole console;
        DebugHud debugHud;
        readonly Dictionary<AssetsType, AbstractPreviewer> previewers;

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public Scene Scene { get; private set; }
        public Viewport Viewport { get; private set; }
        public Node CameraNode { get; private set; }
        public Camera Camera { get; private set; }
        public bool DrawDebugFrame { get; set; }
        public bool HandMode { get; set; }
        public Asset CurrentAsset => currentAsset;
        public AbstractPreviewer CurrentPreviewer => currentPreviewer;

        public UrhoScene(ApplicationOptions opts) : base(opts)
        {
            previewers = new Dictionary<AssetsType, AbstractPreviewer>
            {
                [AssetsType.Model] = new StaticModelPreviewer(this),
                [AssetsType.AnimatedModel] = new AnimatedModelPreviewer(this),
                [AssetsType.Material] = new MaterialPreviewer(this),
                [AssetsType.Scene] = new ScenePreviewer(this),
                [AssetsType.Particle2D] = new Particle2DPreviewer(this),
                [AssetsType.Particle3D] = new Particle3DPreviewer(this),
                [AssetsType.Texture] = new TexturePreviewer(this),
                [AssetsType.UI] = new UIPreviewer(this),
                [AssetsType.RenderPath] = new RenderPathPreviewer(this),
                [AssetsType.AnimationSet2D] = new AnimationSet2DPreviewer(this),
                [AssetsType.Tmx] = new TmxPreviewer(this),
                [AssetsType.Prefab] = new PrefabPreviewer(this),
                [AssetsType.SdfFont] = new SdfFontsPreviewer(this),
            };
        }

        public void CreateScene(string scene)
        {
            // Scene
            Scene = new Scene(Context);
            if (!string.IsNullOrEmpty(scene))
            {
                var xml = ResourceCache.GetFile(scene, false);
                Scene.LoadXml(xml);
                defaultScene = false;
            }
            else
            {
                var octree = Scene.CreateComponent<Octree>();
                // turn off gravity because prefabs may have RigidBody components
                Scene.CreateComponent<PhysicsWorld>().SetGravity(Vector3.Zero);
                defaultScene = true;
            }

            Scene.CreateComponent<DebugRenderer>();
            rootNode = Scene.CreateChild("RootNode");
            rootNode.Position = new Vector3(x: 0, y: 0, z: 0);

            var zoneNode = Scene.CreateChild("Zone");
            var zone = zoneNode.GetComponent<Zone>() ?? zoneNode.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(-10000.0f, 10000.0f));
            zone.AmbientColor = new Color(0.8f, 0.8f, 0.8f);

            // GUI
            errorText = new Text();
            errorText.VerticalAlignment = VerticalAlignment.Center;
            errorText.HorizontalAlignment = HorizontalAlignment.Center;
            errorText.SetColor(new Color(1f, 0.2f, 0.2f));
            errorText.SetFont(font: CoreAssets.Fonts.AnonymousPro, size: 20);
            UI.Root.AddChild(errorText);

            // Camera
            CameraNode = Scene.CreateChild(name: "Camera");
            CameraNode.Position = new Vector3(0, 2, -10);
            CameraNode.Rotation = new Quaternion(10, 0, 0);
            Pitch = 8;
            Camera = CameraNode.CreateComponent<Camera>();
            Input.SetMouseVisible(true, false);


            // Light
            lightNode = CameraNode.CreateChild("DirectionalLight");
            lightNode.Position = new Vector3(0, 20, -5);
            //lightNode.SetDirection(new Vector3(0.2f, 0.0f, 1f));
            lightNode.CreateComponent<Box>();
            light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Point;
            light.CastShadows = true;
            light.Brightness = 1.0f;
            light.Range = 50;


            // Viewport
            Viewport = new Viewport(Context, Scene, Camera, null);
            Renderer.SetViewport(0, Viewport);
            Viewport.RenderPath.Append(CoreAssets.PostProcess.FXAA3);
            ResetClearColor();
        }

        protected override void Start()
        {
            Log.LogLevel = LogLevel.Debug;
            ResourceCache.AutoReloadResources = true;

            var xml = ResourceCache.GetXmlFile("UI/DefaultStyle.xml");
            console = Engine.CreateConsole();
            console.DefaultStyle = xml;
            console.Background.Opacity = 0.8f;
            debugHud = Engine.CreateDebugHud();
            debugHud.DefaultStyle = xml;

            var hud = new MonoDebugHud(this);

            CreateScene(null);

            Input.KeyDown += OnKeyDown;
            Input.MouseWheel += OnMouseWheel;
            Input.MouseMoved += OnMouseMoved;
            Engine.PostRenderUpdate += args => { if (DrawDebugFrame) Renderer.DrawDebugGeometry(false); };
        }

        public void ResetClearColor() => Viewport.SetClearColor(Color.White);

        void OnKeyDown(KeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Esc:
                    //Exit();
                    break;
                case Key.R:
                    RotateRootNode(false);
                    rootNode.Rotation = new Quaternion(0, 0, 0);
                    RotateRootNode(true);
                    break;
                case Key.H:
                    HandMode = !HandMode;
                    break;
                case Key.Space:
                    DrawDebugFrame = !DrawDebugFrame;
                    break;
                case Key.Q:
                    console?.Toggle();
                    break;
                case Key.E:
                    debugHud?.ToggleAll();
                    break;
                case Key.KP_Plus:
                    if (Input.GetKeyDown(Key.B)) Brightness += 0.2f;
                    break;
                case Key.KP_Minus:
                    if (Input.GetKeyDown(Key.B)) Brightness -= 0.2f;
                    break;
            }
        }

        public float Brightness
        {
            get { return light?.Brightness ?? 0; }
            set { if (light != null) light.Brightness = value; }
        }

        void OnMouseMoved(MouseMovedEventArgs e)
        {
            if (!defaultScene)
                return;

            const float mouseSpeed = 0.5f;
            if (Input.GetMouseButtonDown(MouseButton.Left))
            {
                RotateRootNode(false);
                if (HandMode)
                    rootNode.Translate(new Vector3(e.DX, -e.DY, 0) * 0.01f, TransformSpace.World);
                else if (Input.GetKeyDown((Key)'v'))
                    rootNode.Rotate(new Quaternion(-e.DY * mouseSpeed, 0, 0), TransformSpace.World);
                else
                    rootNode.Rotate(new Quaternion(0, -e.DX * mouseSpeed, 0), TransformSpace.Local);
            }
            else
            {
                //RotateRootNode(true);
            }
        }

        void RotateRootNode(bool start)
        {
            if (!rotateActionStarted && start && currentPreviewer?.RotateRootNode == true)
            {
                rootNode.RunActions(new RepeatForever(new RotateBy(1, 0, 10, 0)));
                rotateActionStarted = true;
            }
            else if (rotateActionStarted && (!start || currentPreviewer?.RotateRootNode == false))
            {
                rootNode.RemoveAllActions();
                rotateActionStarted = false;
            }
        }

        void OnMouseWheel(MouseWheelEventArgs args)
        {
            const float scrollSpeed = 1.5f;
            if (Input.GetKeyDown(Key.Z))
            {
                Debug.WriteLine($"Scale: {rootNode.Scale.X},  Wheel: {args.Wheel}");

                if (args.Wheel < 0)
                    rootNode.SetScale(rootNode.Scale.X * 0.5f);
                else if (args.Wheel > 0)
                    rootNode.SetScale(rootNode.Scale.X * 2f);
            }
            else
                CameraNode.Translate(-Vector3.UnitZ * scrollSpeed * args.Wheel * -1);
        }

        public void ShowAsset(Asset asset, IEditor editor)
        {
            if (currentAsset != null && currentAsset.RootDirectory != asset.RootDirectory)
                throw new InvalidOperationException("UrhoApplication must be restarted");

            currentAsset = asset;
            errorText.Value = string.Empty;

            currentPreviewer?.Stop();
            rootNode.RemoveAllComponents();
            rootNode.RemoveAllChildren();
            //rootNode.CreateComponent<Gizmo>();

            var modelNode = rootNode.CreateChild();
            modelNode.SetScale(2f);

            AbstractPreviewer previewer;
            previewers.TryGetValue(asset.Type, out previewer);
            currentPreviewer = previewer;
            if (previewer == null)
            {
                ShowError($"This file is not supported by Urho Previewer");
                return;
            }

            try
            {
                Debug.WriteLine($"Show previewer: {previewer.GetType().Name}");
                previewer.Show(modelNode, asset, editor);
                RotateRootNode(true);
            }
            catch (Exception exc)
            {
                ShowError(exc.ToString());
            }
        }

        void ShowError(string error)
        {
            Debug.WriteLine(LogLevel.Warning, error);
            var errorLines = error.WordWrap(Graphics.Width / 15); //TODO: MeasureString
            errorText.Value = string.Join("\n", errorLines);
        }

        protected override void OnUpdate(float timeStep)
        {
            const float speed = 10;

            if ((!defaultScene && Input.GetMouseButtonDown(MouseButton.Left)) ^ //if we load a custom scene let user to move camera by both left or right mouse buttons
                Input.GetMouseButtonDown(MouseButton.Right))
            {
                const float mouseSensitivity = .1f;
                var mouseMove = Input.MouseMove;
                Yaw += mouseSensitivity * mouseMove.X;
                Pitch += mouseSensitivity * mouseMove.Y;
                Pitch = MathHelper.Clamp(Pitch, -90, 90);
                CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
            }

            if (Input.GetKeyDown(Key.W)) CameraNode.Translate(Vector3.UnitZ * speed * timeStep);
            if (Input.GetKeyDown(Key.S)) CameraNode.Translate(-Vector3.UnitZ * speed * timeStep);
            if (Input.GetKeyDown(Key.A)) CameraNode.Translate(-Vector3.UnitX * speed * timeStep);
            if (Input.GetKeyDown(Key.D)) CameraNode.Translate(Vector3.UnitX * speed * timeStep);
            base.OnUpdate(timeStep);
        }
    }
}
