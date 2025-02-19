using System;
using UnityEngine;


namespace SOSXR.Plet
{
    [Serializable]
    public class ColorSettings
    {
        public string Name;
        public ColorType ColorType;
        public float Saturation = 1f;
        public float Value = 1f;
        public float Alpha = 1f;
        public Color ValuedColor;
    }
}