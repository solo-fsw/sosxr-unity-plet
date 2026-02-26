#if UNITY_EDITOR
using UnityEditor;
#endif
using SOSXR.EnhancedLogger;
using SOSXR.SeaShark;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Header = SOSXR.SeaShark.HeaderAttribute;


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
    public class PletSceneSettings : ScriptableObject
    {
        /// <summary>The active palette providing Base, Tone, and Accent colors for this scene.</summary>
        public Palette Palette;

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

        private readonly int _skyColorShaderId = Shader.PropertyToID("_SkyColor");
        private readonly int _horizonColorShaderId = Shader.PropertyToID("_HorizonColor");
        private readonly int _groundColorShaderId = Shader.PropertyToID("_GroundColor");
        private Palette _previousPalette;
        private ColorProvider[] _allColorProviders = null;


        private void OnValidate()
        {
            if (!IsActiveSceneSettings() || Palette == null)
            {
                return;
            }

            if (_previousPalette == Palette)
            {
                return;
            }

            _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var colorProvider in _allColorProviders)
            {
                colorProvider.Init();
            }

            SetAllSkyboxAndLights();

            _previousPalette = Palette;
        }


        /// <summary>Returns <c>true</c> if this instance is the one currently governing the active scene.</summary>
        public bool IsActiveSceneSettings()
        {
            return PletHelpers.GetPletSceneSettings() == this;
        }


        /// <summary>
        ///     Applies all enabled environment overrides (skybox, ambient light, realtime shadows, fog)
        ///     to Unity's <see cref="UnityEngine.Rendering.RenderSettings"/> using the current palette colors.
        /// </summary>
        public void SetAllSkyboxAndLights()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

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
        ///     Iterates every <see cref="ColorProvider"/> in the scene and resets each one's saturation
        ///     and value from the current palette. Use after swapping the assigned <see cref="Palette"/>.
        /// </summary>
        [Button]
        public void GetColorProvidersSVFromPalette()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

            _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var colorProvider in _allColorProviders)
            {
                colorProvider.GetPaletteSaturationAndValue();
            }
        }


        /// <summary>Re-reads skybox saturation/value from the palette and re-applies all skybox colors.</summary>
        [Button]
        public void GetSkyboxSVFromPalette()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

            SetSVFromPalette(SkyboxSkyHueType, out SkyboxSkySaturation, out SkyboxSkyValue);
            SetSVFromPalette(SkyboxHorizonHueType, out SkyboxHorizonSaturation, out SkyboxHorizonValue);
            SetSVFromPalette(SkyboxGroundHueType, out SkyboxGroundSaturation, out SkyboxGroundValue);
            SetAllSkyboxAndLights();
        }


        /// <summary>Re-reads ambient light saturation/value from the palette and re-applies ambient light colors.</summary>
        [Button]
        public void GetAmbientLightSVFromPalette()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

            SetSVFromPalette(AmbientLightHueType, out AmbientLightSaturation, out AmbientLightValue);
            SetSVFromPalette(AmbientSkyLightHueType, out AmbientSkyLightSaturation, out AmbientSkyLightValue);
            SetSVFromPalette(AmbientEquatorLightHueType, out AmbientEquatorLightSaturation, out AmbientEquatorLightValue);
            SetSVFromPalette(AmbientGroundLightHueType, out AmbientGroundLightSaturation, out AmbientGroundLightValue);
            SetAllSkyboxAndLights();
        }


        /// <summary>Re-reads realtime shadow saturation/value from the palette and re-applies the shadow color.</summary>
        [Button]
        public void GetRealtimeShadowSVFromPalette()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

            SetSVFromPalette(RealtimeShadowHueType, out RealtimeShadowSaturation, out RealtimeShadowValue);
            SetAllSkyboxAndLights();
        }


        /// <summary>Re-reads fog saturation/value from the palette and re-applies the fog color.</summary>
        [Button]
        public void GetFogSVFromPalette()
        {
            if (!IsActiveSceneSettings())
            {
                return;
            }

            SetSVFromPalette(FogHueType, out FogSaturation, out FogValue);
            SetAllSkyboxAndLights();
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
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Flat)
            {
                return;
            }

            AmbientLightColor = ApplyColor(AmbientLightHueType, AmbientLightSaturation, AmbientLightValue);
            RenderSettings.ambientLight = AmbientLightColor;
        }


        private void SetAmbientSkyTriLightColor()
        {
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientSkyLightColor = ApplyColor(AmbientSkyLightHueType, AmbientSkyLightSaturation, AmbientSkyLightValue);
            RenderSettings.ambientSkyColor = AmbientSkyLightColor;
        }


        private void SetAmbientEquatorTriLightColor()
        {
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientEquatorLightColor = ApplyColor(AmbientEquatorLightHueType, AmbientEquatorLightSaturation, AmbientEquatorLightValue);
            RenderSettings.ambientEquatorColor = AmbientEquatorLightColor;
        }


        private void SetAmbientGroundTriLightColor()
        {
            if (!IsActiveSceneSettings() || !ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientGroundLightColor = ApplyColor(AmbientGroundLightHueType, AmbientGroundLightSaturation, AmbientGroundLightValue);
            RenderSettings.ambientGroundColor = AmbientGroundLightColor;
        }


        /// <summary>
        ///     Writes the palette-derived shadow color to <see cref="UnityEngine.Rendering.RenderSettings.subtractiveShadowColor"/>
        ///     when <see cref="ApplyRealtimeShadows"/> is enabled and this instance is active.
        /// </summary>
        public void SetRealtimeShadowColor()
        {
            if (!IsActiveSceneSettings() || !ApplyRealtimeShadows)
            {
                return;
            }

            RealtimeShadowColor = ApplyColor(RealtimeShadowHueType, RealtimeShadowSaturation, RealtimeShadowValue);
            RenderSettings.subtractiveShadowColor = RealtimeShadowColor;
        }


        /// <summary>
        ///     Writes the palette-derived fog color to <see cref="UnityEngine.Rendering.RenderSettings.fogColor"/>
        ///     when fog is enabled in Lighting settings and this instance is active.
        /// </summary>
        public void SetFogColor()
        {
            if (!IsActiveSceneSettings() || !ApplyFog)
            {
                return;
            }

            FogColor = ApplyColor(FogHueType, FogSaturation, FogValue);
            RenderSettings.fogColor = FogColor;
        }


        private void SetSkyboxSkyColor()
        {
            if (!CanApplySkybox())
            {
                return;
            }

            SkyboxSkyColor = ApplyColor(SkyboxSkyHueType, SkyboxSkySaturation, SkyboxSkyValue);
            SkyboxMaterial.SetColor(_skyColorShaderId, SkyboxSkyColor);
        }


        private void SetSkyboxHorizonColor()
        {
            if (!CanApplySkybox())
            {
                return;
            }

            SkyboxHorizonColor = ApplyColor(SkyboxHorizonHueType, SkyboxHorizonSaturation, SkyboxHorizonValue);
            SkyboxMaterial.SetColor(_horizonColorShaderId, SkyboxHorizonColor);
        }


        private void SetSkyboxGroundColor()
        {
            if (!CanApplySkybox())
            {
                return;
            }

            SkyboxGroundColor = ApplyColor(SkyboxGroundHueType, SkyboxGroundSaturation, SkyboxGroundValue);
            SkyboxMaterial.SetColor(_groundColorShaderId, SkyboxGroundColor);
        }


        private bool CanApplySkybox()
        {
            if (!IsActiveSceneSettings() || !ApplySkybox || SkyboxMaterial == null || Palette == null)
            {
                return false;
            }

            if (_skyColorShaderId == 0 || _horizonColorShaderId == 0 || _groundColorShaderId == 0)
            {
                this.Warning("Skybox material missing required shader properties");

                return false;
            }

            return true;
        }


        /// <summary>
        ///     Resolves and assigns the per-scene skybox material, loading it from Resources by convention
        ///     (<c>&lt;sceneName&gt;_plet_skybox</c>) or creating it from the base TriColorSkybox template
        ///     if it does not already exist (editor only).
        /// </summary>
        public void SetSkyboxMaterial()
        {
            if (!IsActiveSceneSettings() || !ApplySkybox)
            {
                return;
            }

            var sceneName = SceneManager.GetActiveScene().name;
            var sceneMatName = $"{sceneName}_plet_skybox";

            if (RenderSettings.skybox != null && RenderSettings.skybox.name == sceneMatName)
            {
                SkyboxMaterial = RenderSettings.skybox;

                return;
            }

            var sceneMat = Resources.Load<Material>(sceneMatName);

            if (sceneMat != null)
            {
                SkyboxMaterial = sceneMat;
                RenderSettings.skybox = sceneMat;
                this.Verbose("Plet scene skybox applied");

                return;
            }

            #if UNITY_EDITOR
            EditorApplication.delayCall += () => { CreateSceneSkyboxMaterial(sceneMatName); };
            #endif
        }


        private void CreateSceneSkyboxMaterial(string sceneMatName)
        {
            var baseMat = Resources.Load<Material>("TriColorSkybox/plet_skybox");

            if (baseMat == null)
            {
                return;
            }

            var targetPath = $"{PletHelpers.FolderPath}/{sceneMatName}.mat";
            var newMat = Instantiate(baseMat);
            newMat.name = sceneMatName;

            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(newMat, targetPath);
            AssetDatabase.SaveAssets();
            #endif

            SkyboxMaterial = newMat;
            RenderSettings.skybox = newMat;
            this.Verbose($"Created new skybox material: {sceneMatName}");
        }


        /// <summary>
        ///     Derives a Unity <see cref="UnityEngine.Color"/> from the palette hue of <paramref name="type"/>
        ///     adjusted by display-range <paramref name="saturation"/> (1–19) and <paramref name="value"/> (1–19),
        ///     then overrides the alpha channel. Saturation and value are linearly interpolated from the
        ///     display range into the configured HSV clamp range.
        /// </summary>
        public Color ApplyColor(HueType type, int saturation, int value, float alpha = 1f)
        {
            var baseColor = GetColorFromPalette(type);
            Color.RGBToHSV(baseColor, out var h, out _, out _);

            // Map display range to clamped HSV values
            var s = Mathf.Lerp(Saturation.Clamp.x, Saturation.Clamp.y,
                (saturation - 1) / (float) (Saturation.DisplayRange.y - 1));

            var v = Mathf.Lerp(Value.Clamp.x, Value.Clamp.y,
                (value - 1) / (float) (Value.DisplayRange.y - 1));

            var newColor = Color.HSVToRGB(h, s, v);
            newColor.a = alpha;

            return newColor;
        }


        /// <summary>
        ///     Converts the raw HSV saturation and value of the palette color for <paramref name="type"/>
        ///     back into display-range integers (1–19 scale) suitable for the inspector sliders.
        /// </summary>
        public Vector2Int GetColorSV(HueType type)
        {
            var baseColor = GetColorFromPalette(type);
            Color.RGBToHSV(baseColor, out _, out var s, out var v);

            // Map HSV values back to display range
            var saturation = Mathf.RoundToInt(
                (s - Saturation.Clamp.x) / (Saturation.Clamp.y - Saturation.Clamp.x) *
                (Saturation.DisplayRange.y - 1)) + 1;

            var value = Mathf.RoundToInt(
                (v - Value.Clamp.x) / (Value.Clamp.y - Value.Clamp.x) *
                (Value.DisplayRange.y - 1)) + 1;

            return new Vector2Int(saturation, value);
        }


        private Color GetColorFromPalette(HueType type)
        {
            if (Palette == null)
            {
                return Color.black;
            }

            return type switch
                   {
                       HueType.Base => Palette.Base,
                       HueType.Tone => Palette.Tone,
                       HueType.Accent => Palette.Accent,
                       _ => Color.white
                   };
        }
    }
}
