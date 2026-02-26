using System;
using System.IO;
using SOSXR.EnhancedLogger;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SOSXR.plet
{
    /// <summary>Static utilities for locating the active <see cref="PletSceneSettings"/> and managing the generated-asset Resources folder path.</summary>
    public static class PletHelpers
    {
        /// <summary>Filename suffix appended to desaturated texture variants (e.g. <c>texture_saturated_50</c>).</summary>
        public static string Suffix => "_saturated_";

        /// <summary>
        ///     Path to the SOSXR Resources folder used for generated assets (palette holders, skybox materials,
        ///     desaturated textures). Creates the directory automatically if it does not exist.
        /// </summary>
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


        /// <summary>
        ///     Loads all <see cref="PletSceneSettings"/> assets from every Resources folder.
        ///     Returns the sole instance if exactly one exists; otherwise returns the instance whose name
        ///     matches the active scene. Creates a new asset in the editor if none are found.
        /// </summary>
        public static PletSceneSettings GetPletSceneSettings()
        {
            var paletteHolders = Resources.LoadAll<PletSceneSettings>("");

            if (paletteHolders.Length == 1)
            {
                return paletteHolders[0];
            }

            var activeScene = SceneManager.GetActiveScene();

            CreatePaletteHolder(paletteHolders);

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


        private static void CreatePaletteHolder(PletSceneSettings[] paletteHolders)
        {
            #if UNITY_EDITOR
            if (paletteHolders.Length == 0)
            {
                Log.Static($"No PaletteSceneSettings found in any of the Resources folders, will create a new one at {FolderPath}.");

                var sceneName = SceneManager.GetActiveScene().name;

                EditorApplication.delayCall += () =>
                {
                    var pletSceneSettings = ScriptableObject.CreateInstance<PletSceneSettings>();
                    var assetPath = Path.Combine(FolderPath, sceneName + ".asset");
                    AssetDatabase.CreateAsset(pletSceneSettings, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                };
            }
            #endif
        }
    }


    /// <summary>Identifies which of the three palette colors to use for a color slot.</summary>
    public enum HueType
    {
        Base,
        Tone,
        Accent
    }


    /// <summary>
    ///     Serializable per-slot color configuration used by <see cref="ColorProvider"/>.
    ///     Tracks the chosen hue type, saturation/value on a 1–19 display scale (10 = palette default),
    ///     alpha, and the resolved final color.
    /// </summary>
    [Serializable]
    public class ColorSettings
    {
        public string Name;
        public HueType HueType;

        /// <summary>Saturation multiplier on a 1–19 display scale (10 = palette default; maps linearly to HSV 0.0075–1.0).</summary>
        public int Saturation;
        /// <summary>Brightness (HSV value) multiplier on a 1–19 display scale (10 = palette default; maps linearly to HSV 0.0075–1.0).</summary>
        public int Value;
        public bool ShowAlpha = true;
        public float Alpha = 1f;
        public Color FinalColor;
    }


    /// <summary>
    ///     Constants defining the editor display range and HSV clamp range for saturation sliders.
    ///     Display range 1–19 maps linearly to HSV values 0.0075–1.0.
    /// </summary>
    public static class Saturation
    {
        public static readonly Vector2Int DisplayRange = new(1, 19);
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }


    /// <summary>
    ///     Constants defining the editor display range and HSV clamp range for value (brightness) sliders.
    ///     Display range 1–19 maps linearly to HSV values 0.0075–1.0.
    /// </summary>
    public static class Value
    {
        public static readonly Vector2Int DisplayRange = new(1, 19);
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }


    /// <summary>
    ///     Serializable configuration for one material slot in a <see cref="TextureProvider"/>.
    ///     Tracks the set of available desaturation-step texture names and the currently selected index.
    /// </summary>
    [Serializable]
    public class TextureSettings
    {
        public string MaterialName;
        /// <summary>Current saturation step index into <see cref="TextureNames"/> (0 = fully desaturated, max = fully saturated).</summary>
        public int Index = TextureProvider.TextureSaturationSteps - 1;
        public int PreviousIndex;
        /// <summary>Resource names of all pre-generated desaturation-step textures, produced by <see cref="Desaturate"/>.</summary>
        public string[] TextureNames = new string[TextureProvider.TextureSaturationSteps];

        [TexturePreview(100)] public Texture2D CurrentTexture;
    }
}