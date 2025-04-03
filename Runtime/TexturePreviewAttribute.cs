using UnityEngine;


namespace SOSXR.plet
{
    public class TexturePreviewAttribute : PropertyAttribute
    {
        public readonly int MaxSize;


        public TexturePreviewAttribute(int maxSize)
        {
            MaxSize = maxSize;
        }
    }
}