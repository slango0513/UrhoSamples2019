using System.IO;

namespace UrhoSharp.Viewer.Core
{
    public class Asset
    {
        public AssetsType Type { get; }
        public string RootDirectory { get; }
        public string RelativePathToAsset { get; }
        public string FullPathToAsset { get; }
        public string OriginalFile { get; }
        public string AssetFileName { get; }

        public Asset(AssetsType type, string rootDirectory, string relativePathToAsset, string originalFile = null)
        {
            Type = type;
            RootDirectory = rootDirectory;
            RelativePathToAsset = relativePathToAsset;
            FullPathToAsset = Path.Combine(rootDirectory, relativePathToAsset);
            AssetFileName = Path.GetFileName(relativePathToAsset);
            OriginalFile = originalFile ?? FullPathToAsset;
        }

        public override string ToString() => $"Asset type={Type}, RootDir={RootDirectory}, RelativePath={RelativePathToAsset}";
    }

    public enum AssetsType
    {
        Unknown,
        Material,
        Model,
        AnimatedModel,
        Texture,
        Particle2D,
        Particle3D,
        Scene,
        UI,
        RenderPath,
        AnimationSet2D,
        Tmx,
        Prefab,
        SdfFont,
        SupportedByAssimp
    }
}
