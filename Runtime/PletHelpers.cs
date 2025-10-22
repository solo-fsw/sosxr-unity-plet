using System;
using System.IO;
using SOSXR.EnhancedLogger;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SOSXR.plet
{
    public static class PletHelpers
    {
        public static string Suffix => "_saturated_";

        public static string FolderPath
        {
            get
            {
                var path = "Assets/_SOSXR/Resources";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }


        public static PletSceneSettings GetPletSceneSettings()
        {
            var paletteHolders = Resources.LoadAll<PletSceneSettings>("");

            if (paletteHolders.Length == 1)
            {
                return paletteHolders[0];
            }

            var activeScene = SceneManager.GetActiveScene();

            if (paletteHolders.Length == 0)
            {
                Log.Static($"No PaletteSceneSettings found in any of the Resources folders, will create a new one at {FolderPath}.");
                
                var pletSceneSettings = ScriptableObject.CreateInstance<PletSceneSettings>();

                var assetPath = Path.Combine(FolderPath, activeScene.name + ".asset");
                AssetDatabase.CreateAsset(pletSceneSettings, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return pletSceneSettings;
            }


            foreach (var paletteHolder in paletteHolders)
            {
                if (paletteHolder.name == activeScene.name)
                {
                    return paletteHolder;
                }
            }

            Log.Static("Multiple PaletteSettings found in Resources folders, but none with the same name as the scene: {0}", activeScene.name);

            return null;
        }
    }


    public enum HueType
    {
        Base,
        Tone,
        Accent
    }


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


    public static class Saturation
    {
        public static readonly Vector2Int DisplayRange = new(1, 19);
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }


    public static class Value
    {
        public static readonly Vector2Int DisplayRange = new(1, 19);
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }


    [Serializable]
    public class TextureSettings
    {
        public string MaterialName;
        public int Index = TextureProvider.TextureSaturationSteps - 1;
        public int PreviousIndex;
        public string[] TextureNames = new string[TextureProvider.TextureSaturationSteps];

        [TexturePreview(100)] public Texture2D CurrentTexture;
    }
}