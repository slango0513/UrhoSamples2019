using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core
{
    /// <summary>
    /// Wrapper for Urho's AssetImporter (Assimp - http://www.assimp.org/)
    /// </summary>
    public class AssimpTool
    {
        static bool installed;
        static string directory;
        static string assimpExecPath;

        // File formats supported by Assimp http://assimp.sourceforge.net/main_features_formats.html
        // NOTE: don't forget to register new formats via attributes in VS (see PreviewerPackage)
        public string[] ModelFormats = { ".obj", ".dae", ".fbx", ".blend", ".3ds", ".ase", ".stl", ".lwo", ".lxo", ".x" };
        public string[] RareModelFormats = { ".glb", ".gltf", ".ifc", ".xgl", ".zgl", ".ply", ".dxf", ".ac", ".ms3d", ".cob", ".scn" };

        public async Task<Asset> ConvertToPrefab(string file, CancellationToken token, bool asPrefab = true)
        {
            InstallIfNeeded();
            string outputDir = Path.Combine(directory, Guid.NewGuid().ToString("N").Substring(0, 7) + "_UrhoData");
            string prefabFileName = "Prefab.xml";
            string assimpArgs = $"{(asPrefab ? "node" : "scene")} \"{file}\" \"{Path.Combine(outputDir, prefabFileName)}\"";

            bool success = await Task.Run(() => ProcessUtils.StartCancellableProcess(assimpExecPath, assimpArgs, token));
            if (!success)
                throw new OperationCanceledException();

            if (!File.Exists(Path.Combine(outputDir, prefabFileName)))
                throw new InvalidOperationException("AssetImporter failed.");

            return new Asset(asPrefab ? AssetsType.Prefab : AssetsType.Scene, outputDir, prefabFileName, file);
        }

        void InstallIfNeeded()
        {
            if (installed)
                return;

            bool isWin = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string assimpExeName = isWin ? "AssetImporter_Win64.exe" : "AssetImporter_macOS";
            directory = Path.Combine(Path.GetTempPath(), "UrhoSharpAssimpTemp");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            assimpExecPath = Path.Combine(directory, assimpExeName);
            GetType().Assembly.CopyEmbeddedResourceTo(assimpExeName, assimpExecPath, true);
            if (!isWin)
                Process.Start("chmod", $"777 \"{assimpExecPath}\"");
            installed = true;
        }
    }
}
