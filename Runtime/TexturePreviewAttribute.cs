using UnityEngine;

namespace SOSXR.plet
{
    /// <summary>
    ///     Property attribute that renders a scaled texture preview directly below the field in the Inspector.
    ///     Handled by <c>TexturePreviewDrawer</c> in the Editor assembly.
    /// </summary>
    public sealed class TexturePreviewAttribute : PropertyAttribute
    {
        /// <summary>Maximum dimension (width or height) of the in-inspector preview image, in pixels.</summary>
        public readonly int MaxSize;

        public TexturePreviewAttribute(int maxSize) => MaxSize = maxSize;
    }
}
