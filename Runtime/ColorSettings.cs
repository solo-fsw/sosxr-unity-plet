using System;
using UnityEngine;


namespace SOSXR.Plet
{
    [Serializable]
    public class ColorSettings
    {
        public string Name;
        public ColorType ColorType;
        public int Saturation = 1;
        public int Value = 1;
        public float Alpha = 1f;
        public Color ValuedColor;
    }
}