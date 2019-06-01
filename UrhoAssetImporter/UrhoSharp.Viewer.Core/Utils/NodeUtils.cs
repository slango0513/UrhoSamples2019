using System.Collections.Generic;
using System.Linq;
using Urho;

namespace UrhoSharp.Viewer.Core.Utils
{
    public static class NodeUtils
    {
        public static void SetScaleBasedOnBoundingBox(this Node node, float referenceScale)
        {
            var maxBoundingBoxSize = GetAllComponentsRecursive(node)
                .Where(c => c is Drawable)
                .Select(c => ((Drawable)c).WorldBoundingBox.Size.Length)
                .DefaultIfEmpty()
                .Max();

            if (maxBoundingBoxSize > 0)
            {
                node.SetScale(referenceScale / maxBoundingBoxSize);
            }
        }

        static IEnumerable<Component> GetAllComponentsRecursive(this Node node)
        {
            foreach (var component in node.Components)
                yield return component;

            foreach (var child in node.Children)
                foreach (var childComponent in GetAllComponentsRecursive(child))
                    yield return childComponent;
        }
    }
}
