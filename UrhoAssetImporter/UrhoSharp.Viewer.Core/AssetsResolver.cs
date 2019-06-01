using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core
{
    public class AssetsResolver
    {
        AssimpTool assimp;

        public bool AssetsImporterFormats { get; set; }
        public bool AssetsImporterRareFormats { get; set; }
        public bool Images { get; set; }

        public AssetsResolver()
        {
            assimp = new AssimpTool();
        }

        public AssetsType ResolveAssetType(string file)
        {
            string extension = (Path.GetExtension(file) ?? string.Empty).ToLowerInvariant();

            // Binary formats:
            if (extension.IsAnyOf(".ani")) return AssetsType.AnimatedModel;
            if (extension.IsAnyOf(".mdl")) return AssetsType.Model;
            if (extension.IsAnyOf(".dds")) return AssetsType.Texture;
            if (extension.IsAnyOf(".sdf") && ReadRootNode(file) == "font") return AssetsType.SdfFont;

            // Images:
            if (Images && extension.IsAnyOf(".png", ".jpg", ".jpeg")) return AssetsType.Texture;

            // File formats supported by Assimp http://assimp.sourceforge.net/main_features_formats.html
            if (AssetsImporterFormats && extension.IsAnyOf(assimp.ModelFormats)) return AssetsType.SupportedByAssimp;
            if (AssetsImporterRareFormats && extension.IsAnyOf(assimp.RareModelFormats)) return AssetsType.SupportedByAssimp;

            // XML
            if (extension.IsAnyOf(".xml", ".pex", ".scml", ".tmx"))
            {
                var node = ReadRootNode(file);
                switch (node)
                {
                    case "material": return AssetsType.Material;
                    case "particleeffect":
                    case "particleemitter": return AssetsType.Particle3D;
                    case "particleemitterconfig": return AssetsType.Particle2D;
                    case "element": return AssetsType.UI;
                    case "texture": return AssetsType.Texture;
                    case "animation": return AssetsType.AnimatedModel;
                    case "renderpath": return AssetsType.RenderPath;
                    case "spriter_data": return AssetsType.AnimationSet2D;
                    case "scene": return AssetsType.Scene;
                    case "node": return AssetsType.Prefab;
                }
            }

            // JSON
            //TODO:

            return AssetsType.Unknown;
        }

        public async Task<Asset> ResolveAsset(string file, CancellationToken token)
        {
            var assetType = ResolveAssetType(file);
            Debug.WriteLine($"AssetType for '{file}' is {assetType}");

            if (assetType == AssetsType.Unknown)
                return null;

            if (assetType == AssetsType.SupportedByAssimp)
                return await assimp.ConvertToPrefab(file, token);

            string rootDir;
            string relativePathToAsset = GetRelativePath(file, out rootDir);

            if (string.IsNullOrEmpty(rootDir) || string.IsNullOrEmpty(relativePathToAsset))
                return null;

            Debug.WriteLine($"'{rootDir}' is recognized as Assets Root Directory");

            return new Asset(assetType, rootDir, relativePathToAsset);
        }

        string GetRelativePath(string absolutePath, out string rootDir)
        {
            rootDir = string.Empty;
            string[] knowRootAssetsDirNames = { "*Data", "Autoload", "*Assets", "Resources" };

            string relativePathToAsset = null;
            foreach (var name in knowRootAssetsDirNames)
            {
                var dirName = name;
                if (dirName.StartsWith("*"))
                    dirName = FsUtils.FindFolder(absolutePath, name.Remove(0, 1));

                if (string.IsNullOrEmpty(dirName))
                    continue;

                if (FsUtils.SplitPath(absolutePath, dirName, out rootDir, out relativePathToAsset))
                    break;
            }
            if (string.IsNullOrEmpty(rootDir))
            {
                rootDir = Path.GetDirectoryName(absolutePath);
                relativePathToAsset = Path.GetFileName(absolutePath);
            }
            return relativePathToAsset;
        }

        static string ReadRootNode(string file)
        {
            try
            {
                using (var reader = XmlReader.Create(file))
                {
                    reader.MoveToContent();
                    return reader.Name?.ToLowerInvariant();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
