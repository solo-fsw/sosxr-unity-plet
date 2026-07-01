#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SOSXR.plet
{
    /// <summary>
    ///     ScriptableObject that acts as the PaletteHolder for a scene (or globally for an entire project).
    ///     Must be placed in a Resources folder. If exactly one instance exists it governs all scenes;
    ///     if multiple instances exist, each must be named exactly after the scene it governs.
    ///     Holds the active <see cref="Palette"/> and optional per-scene overrides for skybox,
    ///     ambient lighting, realtime shadow color, and fog.
    /// </summary>
    [CreateAssetMenu(fileName = "PletSceneSettings", menuName = "SOSXR/plet/PletSceneSettings", order = 1)]
    public sealed class PletSceneSettings : ScriptableObject
    {
        #region Palette
        /// <summary>The active palette providing Base, Tone, and Accent colors for this scene.</summary>
        public Palette Palette;
        #endregion

        #region Skybox
        /// <summary>Whether to apply skybox material and color overrides for this scene.</summary>
        [Header("Skybox Settings")]
        public bool ApplySkybox = false;
        /// <summary>Skybox material instance to assign and tint when <see cref="ApplySkybox"/> is enabled.</summary>
        public Material SkyboxMaterial;
        /// <summary>Palette hue used for the sky portion of the skybox.</summary>
        public HueType SkyboxSkyHueType;
        /// <summary>Sky saturation on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxSkySaturation = 10;
        /// <summary>Sky brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxSkyValue = 10;
        /// <summary>Resolved skybox sky color currently applied (cached for inspector preview).</summary>
        public Color SkyboxSkyColor;

        /// <summary>Palette hue used for the horizon portion of the skybox.</summary>
        public HueType SkyboxHorizonHueType;
        /// <summary>Horizon saturation on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxHorizonSaturation = 10;
        /// <summary>Horizon brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxHorizonValue = 10;
        /// <summary>Resolved skybox horizon color currently applied (cached for inspector preview).</summary>
        public Color SkyboxHorizonColor;

        /// <summary>Palette hue used for the ground portion of the skybox.</summary>
        public HueType SkyboxGroundHueType;
        /// <summary>Ground saturation on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxGroundSaturation = 10;
        /// <summary>Ground brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int SkyboxGroundValue = 10;
        /// <summary>Resolved skybox ground color currently applied (cached for inspector preview).</summary>
        public Color SkyboxGroundColor;
        #endregion

        #region Ambient Light
        /// <summary>Whether to apply ambient light overrides for this scene.</summary>
        [Header("Ambient Light Settings")]
        public bool ApplyAmbientLight = false;

        /// <summary>Palette hue used for flat ambient light (or the sky contribution in tri-light mode).</summary>
        public HueType AmbientLightHueType;
        /// <summary>Ambient light saturation on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientLightSaturation = 10;
        /// <summary>Ambient light brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientLightValue = 10;
        /// <summary>Resolved ambient light color currently applied to <see cref="RenderSettings.ambientLight"/>.</summary>
        public Color AmbientLightColor;

        /// <summary>Palette hue used for the sky ambient contribution in tri-light mode.</summary>
        public HueType AmbientSkyLightHueType;
        /// <summary>Ambient sky saturation on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientSkyLightSaturation = 10;
        /// <summary>Ambient sky brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientSkyLightValue = 10;
        /// <summary>Resolved ambient sky color currently applied to <see cref="RenderSettings.ambientSkyColor"/>.</summary>
        public Color AmbientSkyLightColor;

        /// <summary>Palette hue used for the equator ambient contribution in tri-light mode.</summary>
        public HueType AmbientEquatorLightHueType;
        /// <summary>Ambient equator saturation on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientEquatorLightSaturation = 10;
        /// <summary>Ambient equator brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientEquatorLightValue = 10;
        /// <summary>Resolved ambient equator color currently applied to <see cref="RenderSettings.ambientEquatorColor"/>.</summary>
        public Color AmbientEquatorLightColor;

        /// <summary>Palette hue used for the ground ambient contribution in tri-light mode.</summary>
        public HueType AmbientGroundLightHueType;
        /// <summary>Ambient ground saturation on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientGroundLightSaturation = 10;
        /// <summary>Ambient ground brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int AmbientGroundLightValue = 10;
        /// <summary>Resolved ambient ground color currently applied to <see cref="RenderSettings.ambientGroundColor"/>.</summary>
        public Color AmbientGroundLightColor;
        #endregion

        #region Shadows
        /// <summary>Whether to apply realtime/subtractive shadow color overrides for this scene.</summary>
        [Header("Shadow Settings")]
        public bool ApplyRealtimeShadows = false;
        /// <summary>Palette hue used for the realtime/subtractive shadow color override.</summary>
        public HueType RealtimeShadowHueType;
        /// <summary>Shadow saturation on a 1–19 display scale (10 = palette default).</summary>
        public int RealtimeShadowSaturation = 10;
        /// <summary>Shadow brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int RealtimeShadowValue = 10;
        /// <summary>Resolved shadow color currently applied to <see cref="RenderSettings.subtractiveShadowColor"/>.</summary>
        public Color RealtimeShadowColor;
        #endregion

        #region Fog
        /// <summary>Whether to apply fog color overrides for this scene.</summary>
        [Header("Fog Settings")]
        public bool ApplyFog = false;
        /// <summary>Palette hue used for the fog color override.</summary>
        public HueType FogHueType;
        /// <summary>Fog saturation on a 1–19 display scale (10 = palette default).</summary>
        public int FogSaturation = 10;
        /// <summary>Fog brightness/value on a 1–19 display scale (10 = palette default).</summary>
        public int FogValue = 10;
        /// <summary>Resolved fog color currently applied to <see cref="RenderSettings.fogColor"/>.</summary>
        public Color FogColor;
        #endregion

        #region Private Fields
        private static readonly int s_skyColorShaderId = Shader.PropertyToID("_SkyColor");
        private static readonly int s_horizonColorShaderId = Shader.PropertyToID("_HorizonColor");
        private static readonly int s_groundColorShaderId = Shader.PropertyToID("_GroundColor");
        private static readonly Dictionary<string, Material> s_materialCache = new();

        [HideInInspector][SerializeField] private Palette _previousPalette;
        private ColorProvider[] _allColorProviders;
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            if (!IsActiveSceneSettings() || Palette == null || _previousPalette == Palette)
            {
                return;
            }

            // Defer expensive operations to avoid editor lag during property changes
#if UNITY_EDITOR
            EditorApplication.delayCall += ApplyPaletteChange;
#endif
            _previousPalette = Palette;
        }

#if UNITY_EDITOR
        private void ApplyPaletteChange()
        {
            if (this == null)
                return;

            _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include);

            foreach (var colorProvider in _allColorProviders)
            {
                colorProvider?.Init();
            }

            SetAllSkyboxAndLights();
        }
#endif
        #endregion

        #region Public Methods
        /// <summary>Returns <c>true</c> if this instance is the one currently governing the active scene.</summary>
        public bool IsActiveSceneSettings() => PletHelpers.GetPletSceneSettings() == this;

        /// <summary>
        ///     Applies all enabled environment overrides (skybox, ambient light, realtime shadows, fog)
        ///     to Unity's <see cref="RenderSettings"/> using the current palette colors.
        /// </summary>
        public void SetAllSkyboxAndLights()
        {
            if (!IsActiveSceneSettings())
                return;

            SetSkyboxMaterial();
            SetSkyboxColors();
            SetAmbientLightColors();
            SetRealtimeShadowColor();
            SetFogColor();

#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }

        /// <summary>
        ///     Derives a Unity <see cref="Color"/> from the palette hue of <paramref name="type"/>
        ///     adjusted by display-range <paramref name="saturation"/> (1–19) and <paramref name="value"/> (1–19),
        ///     then overrides the alpha channel.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when saturation or value is outside the valid range.</exception>
        public Color ApplyColor(HueType type, int saturation, int value, float alpha = 1f)
        {
            if (saturation is < 1 or > 19)
                throw new ArgumentOutOfRangeException(nameof(saturation), "Must be between 1 and 19.");
            if (value is < 1 or > 19)
                throw new ArgumentOutOfRangeException(nameof(value), "Must be between 1 and 19.");

            var baseColor = GetColorFromPalette(type);
            Color.RGBToHSV(baseColor, out float h, out _, out _);

            float s = DisplayToHsv(saturation, Saturation.Clamp);
            float v = DisplayToHsv(value, Value.Clamp);

            var color = Color.HSVToRGB(h, s, v);
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        /// <summary>
        ///     Converts the raw HSV saturation and value of the palette color for <paramref name="type"/>
        ///     back into display-range integers (1–19 scale).
        /// </summary>
        public Vector2Int GetColorSV(HueType type)
        {
            var baseColor = GetColorFromPalette(type);
            Color.RGBToHSV(baseColor, out _, out float s, out float v);

            int saturation = HsvToDisplay(s, Saturation.Clamp);
            int value = HsvToDisplay(v, Value.Clamp);

            return new Vector2Int(saturation, value);
        }

        private static float DisplayToHsv(int displayValue, Vector2 clampRange)
        {
            return Mathf.Lerp(clampRange.x, clampRange.y,
                (displayValue - 1) / (float)(19 - 1));
        }

        private static int HsvToDisplay(float hsvValue, Vector2 clampRange)
        {
            return Mathf.RoundToInt(
                (hsvValue - clampRange.x) / (clampRange.y - clampRange.x) *
                (19 - 1)) + 1;
        }

        /// <summary>
        ///     Resolves and assigns the per-scene skybox material.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when base skybox material is not found.</exception>
        public void SetSkyboxMaterial()
        {
            if (!IsActiveSceneSettings() || !ApplySkybox)
                return;

            string sceneName = SceneManager.GetActiveScene().name;
            string sceneMatName = $"{sceneName}_plet_skybox";

            if (RenderSettings.skybox == SkyboxMaterial)
                return;

            Material sceneMat;
            if (!s_materialCache.TryGetValue(sceneMatName, out sceneMat))
            {
                sceneMat = Resources.Load<Material>(sceneMatName);
                if (sceneMat != null)
                    s_materialCache[sceneMatName] = sceneMat;
            }

            if (sceneMat != null)
            {
                SkyboxMaterial = sceneMat;
                RenderSettings.skybox = sceneMat;
                return;
            }

#if UNITY_EDITOR
            EditorApplication.delayCall += () => CreateSceneSkyboxMaterial(sceneMatName);
#endif
        }
        #endregion

        #region Button Methods
        [Button]
        public void GetColorProvidersSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            if (_allColorProviders == null || _allColorProviders.Length == 0)
            {
                _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include);
            }

            for (int i = 0; i < _allColorProviders.Length; i++)
            {
                _allColorProviders[i]?.GetPaletteSaturationAndValue();
            }
        }

        [Button]
        public void GetSkyboxSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            SetSVFromPalette(SkyboxSkyHueType, out SkyboxSkySaturation, out SkyboxSkyValue);
            SetSVFromPalette(SkyboxHorizonHueType, out SkyboxHorizonSaturation, out SkyboxHorizonValue);
            SetSVFromPalette(SkyboxGroundHueType, out SkyboxGroundSaturation, out SkyboxGroundValue);
            SetAllSkyboxAndLights();
        }

        [Button]
        public void GetAmbientLightSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            SetSVFromPalette(AmbientLightHueType, out AmbientLightSaturation, out AmbientLightValue);
            SetSVFromPalette(AmbientSkyLightHueType, out AmbientSkyLightSaturation, out AmbientSkyLightValue);
            SetSVFromPalette(AmbientEquatorLightHueType, out AmbientEquatorLightSaturation, out AmbientEquatorLightValue);
            SetSVFromPalette(AmbientGroundLightHueType, out AmbientGroundLightSaturation, out AmbientGroundLightValue);
            SetAllSkyboxAndLights();
        }

        [Button]
        public void GetRealtimeShadowSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            SetSVFromPalette(RealtimeShadowHueType, out RealtimeShadowSaturation, out RealtimeShadowValue);
            SetAllSkyboxAndLights();
        }

        [Button]
        public void GetFogSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            SetSVFromPalette(FogHueType, out FogSaturation, out FogValue);
            SetAllSkyboxAndLights();
        }
        #endregion

        #region Private Methods
        private void SetSVFromPalette(HueType hueType, out int saturation, out int value)
        {
            var sv = GetColorSV(hueType);
            saturation = sv.x;
            value = sv.y;
        }

        private void SetSkyboxColors()
        {
            SetSkyboxSkyColor();
            SetSkyboxHorizonColor();
            SetSkyboxGroundColor();
        }

        private void SetAmbientLightColors()
        {
            SetAmbientLightColor();
            SetAmbientSkyTriLightColor();
            SetAmbientEquatorTriLightColor();
            SetAmbientGroundTriLightColor();
        }

        private void SetAmbientLightColor()
        {
            SetAmbientColor(AmbientMode.Flat, AmbientLightHueType, AmbientLightSaturation, AmbientLightValue,
                ref AmbientLightColor, color => RenderSettings.ambientLight = color);
        }

        private void SetAmbientSkyTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientSkyLightHueType, AmbientSkyLightSaturation, AmbientSkyLightValue,
                ref AmbientSkyLightColor, color => RenderSettings.ambientSkyColor = color);
        }

        private void SetAmbientEquatorTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientEquatorLightHueType, AmbientEquatorLightSaturation, AmbientEquatorLightValue,
                ref AmbientEquatorLightColor, color => RenderSettings.ambientEquatorColor = color);
        }

        private void SetAmbientGroundTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientGroundLightHueType, AmbientGroundLightSaturation, AmbientGroundLightValue,
                ref AmbientGroundLightColor, color => RenderSettings.ambientGroundColor = color);
        }

        private void SetAmbientColor(AmbientMode requiredMode, HueType hueType, int saturation, int value,
            ref Color colorRef, System.Action<Color> setter)
        {
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != requiredMode)
                return;

            colorRef = ApplyColor(hueType, saturation, value);
            setter(colorRef);
        }

        public void SetRealtimeShadowColor()
        {
            if (!IsActiveSceneSettings() || !ApplyRealtimeShadows)
                return;

            RealtimeShadowColor = ApplyColor(RealtimeShadowHueType, RealtimeShadowSaturation, RealtimeShadowValue);
            RenderSettings.subtractiveShadowColor = RealtimeShadowColor;
        }

        public void SetFogColor()
        {
            if (!IsActiveSceneSettings() || !ApplyFog)
                return;

            FogColor = ApplyColor(FogHueType, FogSaturation, FogValue);
            RenderSettings.fogColor = FogColor;
        }

        private void SetSkyboxSkyColor() =>
            SetSkyboxColor(SkyboxSkyHueType, SkyboxSkySaturation, SkyboxSkyValue, ref SkyboxSkyColor, s_skyColorShaderId);

        private void SetSkyboxHorizonColor() =>
            SetSkyboxColor(SkyboxHorizonHueType, SkyboxHorizonSaturation, SkyboxHorizonValue, ref SkyboxHorizonColor, s_horizonColorShaderId);

        private void SetSkyboxGroundColor() =>
            SetSkyboxColor(SkyboxGroundHueType, SkyboxGroundSaturation, SkyboxGroundValue, ref SkyboxGroundColor, s_groundColorShaderId);

        private void SetSkyboxColor(HueType hueType, int saturation, int value, ref Color colorRef, int shaderPropertyId)
        {
            if (!CanApplySkybox())
                return;

            colorRef = ApplyColor(hueType, saturation, value);
            SkyboxMaterial.SetColor(shaderPropertyId, colorRef);
        }

        private bool CanApplySkybox()
        {
            if (!IsActiveSceneSettings() || !ApplySkybox || SkyboxMaterial == null || Palette == null)
                return false;

            if (!SkyboxMaterial.HasProperty(s_skyColorShaderId) ||
                !SkyboxMaterial.HasProperty(s_horizonColorShaderId) ||
                !SkyboxMaterial.HasProperty(s_groundColorShaderId))
            {
                Debug.LogWarning(
                    "Skybox material missing required shader properties (_SkyColor, _HorizonColor, or _GroundColor).",
                    SkyboxMaterial);
                return false;
            }

            return true;
        }

        private void CreateSceneSkyboxMaterial(string sceneMatName)
        {
            var baseMat = Resources.Load<Material>("TriColorSkybox/plet_skybox");

            if (baseMat == null)
            {
                throw new InvalidOperationException(
                    "Base skybox material 'TriColorSkybox/plet_skybox' not found in Resources. " +
                    "Cannot create scene skybox material.");
            }

            string targetPath = $"{PletHelpers.FolderPath}/{sceneMatName}.mat";
            var newMat = Instantiate(baseMat);
            newMat.name = sceneMatName;

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(newMat, targetPath);
            AssetDatabase.SaveAssets();
#endif

            SkyboxMaterial = newMat;
            RenderSettings.skybox = newMat;
            s_materialCache[sceneMatName] = newMat;
        }

        private Color GetColorFromPalette(HueType type) => Palette == null
            ? Color.black
            : type switch
            {
                HueType.Base => Palette.Base,
                HueType.Tone => Palette.Tone,
                HueType.Accent => Palette.Accent,
                _ => Color.white
            };
        #endregion
    }
}
