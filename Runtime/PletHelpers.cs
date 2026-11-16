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

        /// <summary>Resources-relative folder for generated desaturated textures (load with <c>Resources.Load</c>).</summary>
        public const string TextureResourcesFolder = "Plet/Textures";

        /// <summary>Name used for generated assets when the active scene has no name (untitled scene).</summary>
        public const string UntitledSceneName = "Untitled";

        private static bool s_settingsCreationPending;

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
#if UNITY_EDITOR
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
#endif
                    s_folderPath = path;
                    s_folderPathInitialized = true;
                }
                return s_folderPath;
            }
        }

        /// <summary>Project path of the folder where desaturated textures are saved (inside <see cref="FolderPath"/>).</summary>
        public static string TextureFolderPath => $"{FolderPath}/{TextureResourcesFolder}";

        /// <summary>Returns the Resources path (for <c>Resources.Load</c>) of a generated texture called <paramref name="textureName"/>.</summary>
        public static string TextureResourcePath(string textureName) => $"{TextureResourcesFolder}/{textureName}";

        /// <summary>Returns the asset name of the desaturated variant of <paramref name="baseName"/> at <paramref name="saturationPercentage"/>.</summary>
        public static string DesaturatedTextureName(string baseName, int saturationPercentage) => $"{baseName}{Suffix}{saturationPercentage}";

        /// <summary>Number of asset GUID characters added to generated texture names (see <see cref="DesaturatedTextureBaseName(string, string)"/>).</summary>
        public const int TextureGuidPrefixLength = 8;

        /// <summary>
        ///     Returns the base name for the desaturated variants of <paramref name="textureName"/>: the name plus the first
        ///     <see cref="TextureGuidPrefixLength"/> characters of <paramref name="assetGuid"/> (e.g. <c>wood_1a2b3c4d</c>).
        ///     All variants share one flat folder, so the GUID keeps same-named textures from different folders apart.
        ///     Returns <paramref name="textureName"/> unchanged when <paramref name="assetGuid"/> is null or empty.
        /// </summary>
        public static string DesaturatedTextureBaseName(string textureName, string assetGuid) =>
            string.IsNullOrEmpty(assetGuid)
                ? textureName
                : $"{textureName}_{assetGuid.Substring(0, Math.Min(TextureGuidPrefixLength, assetGuid.Length))}";

        /// <summary>
        ///     Returns the base name for the desaturated variants of <paramref name="texture"/>. In the editor this includes
        ///     a short prefix of the texture's asset GUID; for textures that are not assets (and in builds) it is the texture name.
        /// </summary>
        public static string DesaturatedTextureBaseName(Texture2D texture)
        {
            if (texture == null) return null;

            string guid = null;
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (!string.IsNullOrEmpty(assetPath))
                guid = AssetDatabase.AssetPathToGUID(assetPath);
#endif
            return DesaturatedTextureBaseName(texture.name, guid);
        }

        /// <summary>
        ///     Returns the saturation percentage (0–100) for desaturation step <paramref name="step"/>
        ///     out of <paramref name="stepCount"/> steps.
        /// </summary>
        public static int SaturationPercentageForStep(int step, int stepCount) =>
            stepCount <= 1 ? 100 : Mathf.RoundToInt((float)step / (stepCount - 1) * 100);

        /// <summary>Returns <paramref name="sceneName"/>, or <see cref="UntitledSceneName"/> when it is null or empty.</summary>
        public static string GetSafeSceneName(string sceneName) =>
            string.IsNullOrEmpty(sceneName) ? UntitledSceneName : sceneName;

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
            // OnValidate can call this many times in a row. Schedule at most one creation.
            if (paletteHolders.Length != 0 || s_settingsCreationPending || Application.isPlaying)
                return;

            s_settingsCreationPending = true;

            string sceneName = GetSafeSceneName(SceneManager.GetActiveScene().name);
            string assetPath = $"{FolderPath}/{sceneName}.asset";

            Debug.Log($"No PletSceneSettings found in any of the Resources folders, will create a new one at {assetPath}.");

            EditorApplication.delayCall += () =>
            {
                s_settingsCreationPending = false;

                // Something may have created one in the meantime. Never overwrite an existing asset.
                if (Resources.LoadAll<PletSceneSettings>("").Length != 0 ||
                    AssetDatabase.LoadAssetAtPath<PletSceneSettings>(assetPath) != null)
                    return;

                var pletSceneSettings = ScriptableObject.CreateInstance<PletSceneSettings>();
                AssetDatabase.CreateAsset(pletSceneSettings, AssetDatabase.GenerateUniqueAssetPath(assetPath));
                AssetDatabase.SaveAssets();
                InvalidateCache();
            };
#endif
        }
    }

    /// <summary>Identifies which of the three palette colors to use for a color slot.</summary>
    public enum HueType
    {
        /// <summary>The base palette color.</summary>
        Base,
        /// <summary>The tone palette color.</summary>
        Tone,
        /// <summary>The accent palette color.</summary>
        Accent
    }

    /// <summary>
    ///     Serializable per-slot color configuration used by <see cref="ColorProvider"/>.
    ///     Tracks the chosen hue type, saturation/value on a 1–19 display scale (absolute HSV, not relative to the palette colour),
    ///     alpha, and the resolved final color.
    /// </summary>
    [Serializable]
    public sealed class ColorSettings
    {
        /// <summary>Display name of this color slot.</summary>
        public string Name;
        /// <summary>The palette color (Base, Tone or Accent) this slot is derived from.</summary>
        public HueType HueType;

        /// <summary>
        ///     Saturation on a 1–19 display scale; maps linearly to absolute HSV saturation 0.0075–1.0 (10 ≈ 0.5).
        ///     Use "Get Palette Saturation and Value" to match the palette colour's own saturation.
        /// </summary>
        public int Saturation = 10;
        /// <summary>
        ///     Brightness on a 1–19 display scale; maps linearly to absolute HSV value 0.0075–1.0 (10 ≈ 0.5).
        ///     Use "Get Palette Saturation and Value" to match the palette colour's own value.
        /// </summary>
        public int Value = 10;
        /// <summary>Whether the alpha value is shown for this slot.</summary>
        public bool ShowAlpha = true;
        /// <summary>Alpha (opacity) of the resolved color, from 0 to 1.</summary>
        public float Alpha = 1f;
        /// <summary>The resolved color for this slot.</summary>
        public Color FinalColor = Color.white;
    }

    /// <summary>
    ///     Constants defining the editor display range and HSV clamp range for saturation sliders.
    ///     Display range 1–19 maps linearly to HSV values 0.0075–1.0.
    /// </summary>
    public static class Saturation
    {
        /// <summary>The editor display range (1 to 19) of saturation sliders.</summary>
        public static readonly Vector2Int DisplayRange = new(1, 19);
        /// <summary>The HSV saturation range that the display range maps to (0.0075 to 1).</summary>
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }

    /// <summary>
    ///     Constants defining the editor display range and HSV clamp range for value (brightness) sliders.
    ///     Display range 1–19 maps linearly to HSV values 0.0075–1.0.
    /// </summary>
    public static class Value
    {
        /// <summary>The editor display range (1 to 19) of value (brightness) sliders.</summary>
        public static readonly Vector2Int DisplayRange = new(1, 19);
        /// <summary>The HSV value range that the display range maps to (0.0075 to 1).</summary>
        public static readonly Vector2 Clamp = new(0.0075f, 1.0f);
    }

    /// <summary>
    ///     Serializable configuration for one material slot in a <see cref="TextureProvider"/>.
    ///     Tracks the set of available desaturation-step texture names and the currently selected index.
    /// </summary>
    [Serializable]
    public sealed class TextureSettings
    {
        /// <summary>Name of the material in the slot this entry belongs to.</summary>
        public string MaterialName;
        /// <summary>Index of the material slot on the renderer this entry belongs to.</summary>
        public int MaterialIndex;
        /// <summary>Current saturation step index into <see cref="TextureNames"/> (0 = fully desaturated, max = fully saturated).</summary>
        public int Index = TextureProvider.TextureSaturationSteps - 1;
        /// <summary>The <see cref="Index"/> value that was last applied, used to detect changes.</summary>
        public int PreviousIndex;
        /// <summary>Resources paths of all pre-generated desaturation-step textures (index 0 = greyscale).</summary>
        public string[] TextureNames = new string[TextureProvider.TextureSaturationSteps];

        /// <summary>The texture currently applied to this slot.</summary>
        [TexturePreview(100)] public Texture2D CurrentTexture;
    }
}
