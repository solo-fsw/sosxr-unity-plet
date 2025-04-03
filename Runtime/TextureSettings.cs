using System;
using UnityEngine;


namespace SOSXR.plet
{
    [Serializable]
    public class TextureSettings
    {
        public string MaterialName;
        public int Index = TextureProvider.TextureSaturationSteps - 1;
        public int PreviousIndex;
        public string[] TextureNames = new string[TextureProvider.TextureSaturationSteps];

        [TexturePreview(100)]
        public Texture2D CurrentTexture;
    }
}