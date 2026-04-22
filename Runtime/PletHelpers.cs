using System;
using System.IO;
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

        private static PletSceneSettings s_cachedSettings;
        private static string s_cachedSceneName;

        private static string s_folderPath;
        private static bool s_folderPathInitialized;

        /// <summary>
        ///     Path to the SOSXR Resources folder used for generated assets (palette holders, skybox materials,
        ///     desaturated textures). Creates the directory automatically if it does not exist.
        /// </summary>
        public static string FolderPath
        {
            get
            {
                if (!s_folderPathInitialized)
                {
                    const string path = "Assets/_SOSXR/Resources";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    s_folderPath = path;
                    s_folderPathInitialized = true;
                }
                return s_folderPath;
            }
        }

        /// <summary>
        ///     Loads all <see cref="PletSceneSettings"/> assets from every Resources folder.
        ///     Returns the sole instance if exactly one exists; otherwise returns the instance whose name
        ///     matches the active scene. Creates a new asset in the editor if none are found.
        ///     Results are cached per scene to avoid repeated <see cref="Resources.LoadAll{T}"/> calls.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when multiple settings exist but none match the scene name.</exception>
        public static PletSceneSettings GetPletSceneSettings()
        {
            var activeScene = SceneManager.GetActiveScene();

            // Validate cache - check if scene is still valid and loaded
            if (s_cachedSettings != null &&
                s_cachedSceneName == activeScene.name &&
                activeScene.IsValid() &&
                activeScene.isLoaded)
            {
                return s_cachedSettings;
            }

            var paletteHolders = Resources.LoadAll<PletSceneSettings>("");

            if (paletteHolders.Length == 1)
            {
                s_cachedSettings = paletteHolders[0];
                s_cachedSceneName = activeScene.name;

                return s_cachedSettings;
            }

            CreatePaletteHolder(paletteHolders);

            foreach (var paletteHolder in paletteHolders)
            {
                if (paletteHolder != null && paletteHolder.name == activeScene.name)
                {
                    s_cachedSettings = paletteHolder;
                    s_cachedSceneName = activeScene.name;

                    return s_cachedSettings;
                }
            }

            if (paletteHolders.Length > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple PaletteSettings found in Resources folders, but none with the same name as the scene: {activeScene.name}");
            }

            return null;
        }

        /// <summary>Clears the cached <see cref="PletSceneSettings"/> so the next call to <see cref="GetPletSceneSettings"/> will re-scan.</summary>
        public static void InvalidateCache()
        {
            s_cachedSettings = null;
            s_cachedSceneName = null;
        }

        private static void CreatePaletteHolder(PletSceneSettings[] paletteHolders)
        {
#if UNITY_EDITOR
            if (paletteHolders.Length != 0)
                return;

            Debug.Log($"No PaletteSceneSettings found in any of the Resources folders, will create a new one at {FolderPath}.");

            string sceneName = SceneManager.GetActiveScene().name;

            EditorApplication.delayCall += () =>
            {
                var pletSceneSettings = ScriptableObject.CreateInstance<PletSceneSettings>();
                string assetPath = Path.Combine(FolderPath, $"{sceneName}.asset");
                AssetDatabase.CreateAsset(pletSceneSettings, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
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
    public sealed class ColorSettings
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
    public sealed class TextureSettings
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
