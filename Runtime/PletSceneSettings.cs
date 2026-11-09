#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
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
        private static readonly HashSet<string> s_pendingSkyboxMaterials = new();

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

            if (_allColorProviders.Length > 0)
            {
                Undo.RecordObjects(_allColorProviders, "Apply plet palette");
            }

            foreach (var colorProvider in _allColorProviders)
            {
                if (colorProvider == null) continue;
                colorProvider.Init();
                EditorUtility.SetDirty(colorProvider);
            }

            SetAllSkyboxAndLights();
        }
#endif
        #endregion

        #region Public Methods
        /// <summary>Returns <c>true</c> if this instance is the one currently governing the active scene.</summary>
        /// <remarks>Returns <c>false</c> (instead of throwing) when several settings exist and none matches the scene.</remarks>
        public bool IsActiveSceneSettings()
        {
            try
            {
                return PletHelpers.GetPletSceneSettings() == this;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

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
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);

            SceneView.RepaintAll();
#endif
        }

        /// <summary>
        ///     Derives a Unity <see cref="Color"/> from the palette hue of <paramref name="type"/>
        ///     adjusted by display-range <paramref name="saturation"/> (1–19) and <paramref name="value"/> (1–19),
        ///     then overrides the alpha channel.
        /// </summary>
        /// <remarks>
        ///     Out-of-range <paramref name="saturation"/> and <paramref name="value"/> are clamped to 1–19
        ///     (for example the 0 of a freshly added slot), so this never throws.
        /// </remarks>
        public Color ApplyColor(HueType type, int saturation, int value, float alpha = 1f)
        {
            saturation = Mathf.Clamp(saturation, Saturation.DisplayRange.x, Saturation.DisplayRange.y);
            value = Mathf.Clamp(value, Value.DisplayRange.x, Value.DisplayRange.y);

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
            int display = Mathf.RoundToInt(
                (hsvValue - clampRange.x) / (clampRange.y - clampRange.x) *
                (19 - 1)) + 1;

            return Mathf.Clamp(display, 1, 19);
        }

        /// <summary>
        ///     Resolves and assigns the per-scene skybox material.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when base skybox material is not found.</exception>
        public void SetSkyboxMaterial()
        {
            if (!IsActiveSceneSettings() || !ApplySkybox)
                return;

            string sceneMatName = GetSkyboxMaterialName(SceneManager.GetActiveScene().name);

            if (SkyboxMaterial != null && RenderSettings.skybox == SkyboxMaterial)
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
                AssignSkyboxMaterial(sceneMat);
                return;
            }

#if UNITY_EDITOR
            // Called from OnValidate and inspector changes: schedule at most one creation per material.
            if (Application.isPlaying || !s_pendingSkyboxMaterials.Add(sceneMatName))
                return;

            EditorApplication.delayCall += () =>
            {
                s_pendingSkyboxMaterials.Remove(sceneMatName);

                if (this != null)
                    CreateSceneSkyboxMaterial(sceneMatName);
            };
#endif
        }

        /// <summary>Returns the name of the generated per-scene skybox material for <paramref name="sceneName"/>.</summary>
        public static string GetSkyboxMaterialName(string sceneName) =>
            $"{PletHelpers.GetSafeSceneName(sceneName)}_plet_skybox";
        #endregion

        #region Button Methods
        [Button]
        public void GetColorProvidersSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            // Always re-scan: providers may have been added or destroyed since the last call.
            _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include);

#if UNITY_EDITOR
            if (!Application.isPlaying && _allColorProviders.Length > 0)
                Undo.RecordObjects(_allColorProviders, "Get palette saturation and value");
#endif

            for (int i = 0; i < _allColorProviders.Length; i++)
            {
                if (_allColorProviders[i] == null) continue;

                _allColorProviders[i].GetPaletteSaturationAndValue();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.SetDirty(_allColorProviders[i]);
#endif
            }
        }

        [Button]
        public void GetSkyboxSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            RecordSelf("GetSkyboxSVFromPalette");

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

            RecordSelf("GetAmbientLightSVFromPalette");

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

            RecordSelf("GetRealtimeShadowSVFromPalette");

            SetSVFromPalette(RealtimeShadowHueType, out RealtimeShadowSaturation, out RealtimeShadowValue);
            SetAllSkyboxAndLights();
        }

        [Button]
        public void GetFogSVFromPalette()
        {
            if (!IsActiveSceneSettings())
                return;

            RecordSelf("GetFogSVFromPalette");

            SetSVFromPalette(FogHueType, out FogSaturation, out FogValue);
            SetAllSkyboxAndLights();
        }
        #endregion

        #region Private Methods
        private void RecordSelf(string undoName)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.RecordObject(this, undoName);
#endif
        }

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
                ref AmbientLightColor, RenderSettings.ambientLight, color => RenderSettings.ambientLight = color);
        }

        private void SetAmbientSkyTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientSkyLightHueType, AmbientSkyLightSaturation, AmbientSkyLightValue,
                ref AmbientSkyLightColor, RenderSettings.ambientSkyColor, color => RenderSettings.ambientSkyColor = color);
        }

        private void SetAmbientEquatorTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientEquatorLightHueType, AmbientEquatorLightSaturation, AmbientEquatorLightValue,
                ref AmbientEquatorLightColor, RenderSettings.ambientEquatorColor, color => RenderSettings.ambientEquatorColor = color);
        }

        private void SetAmbientGroundTriLightColor()
        {
            SetAmbientColor(AmbientMode.Trilight, AmbientGroundLightHueType, AmbientGroundLightSaturation, AmbientGroundLightValue,
                ref AmbientGroundLightColor, RenderSettings.ambientGroundColor, color => RenderSettings.ambientGroundColor = color);
        }

        private void SetAmbientColor(AmbientMode requiredMode, HueType hueType, int saturation, int value,
            ref Color colorRef, Color current, Action<Color> setter)
        {
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != requiredMode)
                return;

            colorRef = ApplyColor(hueType, saturation, value);
            SetRenderSetting(current, colorRef, setter);
        }

        public void SetRealtimeShadowColor()
        {
            if (!IsActiveSceneSettings() || !ApplyRealtimeShadows)
                return;

            RealtimeShadowColor = ApplyColor(RealtimeShadowHueType, RealtimeShadowSaturation, RealtimeShadowValue);
            SetRenderSetting(RenderSettings.subtractiveShadowColor, RealtimeShadowColor, color => RenderSettings.subtractiveShadowColor = color);
        }

        public void SetFogColor()
        {
            if (!IsActiveSceneSettings() || !ApplyFog)
                return;

            FogColor = ApplyColor(FogHueType, FogSaturation, FogValue);
            SetRenderSetting(RenderSettings.fogColor, FogColor, color => RenderSettings.fogColor = color);
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

            if (SkyboxMaterial.GetColor(shaderPropertyId) == colorRef)
                return;

            SkyboxMaterial.SetColor(shaderPropertyId, colorRef);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(SkyboxMaterial);
#endif
        }

        /// <summary>
        ///     Applies <paramref name="color"/> to a <see cref="RenderSettings"/> value through <paramref name="setter"/>.
        ///     RenderSettings are saved with the scene, so in edit mode the scene is marked dirty when the value changes.
        /// </summary>
        private static void SetRenderSetting(Color current, Color color, Action<Color> setter)
        {
            if (current == color)
                return;

            setter(color);
            MarkActiveSceneDirty();
        }

        private void AssignSkyboxMaterial(Material material)
        {
            SkyboxMaterial = material;

            if (RenderSettings.skybox == material)
                return;

            RenderSettings.skybox = material;
            MarkActiveSceneDirty();
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }

        private static void MarkActiveSceneDirty()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid())
                EditorSceneManager.MarkSceneDirty(scene);
#endif
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

            Material newMat = null;
#if UNITY_EDITOR
            // Never overwrite an existing material (CreateAsset would replace it).
            newMat = AssetDatabase.LoadAssetAtPath<Material>(targetPath);

            if (newMat == null)
            {
                newMat = Instantiate(baseMat);
                newMat.name = sceneMatName;
                AssetDatabase.CreateAsset(newMat, targetPath);
                AssetDatabase.SaveAssets();
            }
#endif
            if (newMat == null)
                return;

            AssignSkyboxMaterial(newMat);
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
