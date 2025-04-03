using System;
using UnityEngine;


namespace SOSXR.plet
{
    [Serializable]
    public class ColorSettings
    {
        public string Name;
        public HueType HueType;

        public int Saturation;
        public int Value;
        public bool ShowAlpha = true;
        public float Alpha = 1f;
        public Color FinalColor;
    }
}