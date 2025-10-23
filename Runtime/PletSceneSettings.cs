using SOSXR.EnhancedLogger;
using SOSXR.SeaShark;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Header = SOSXR.SeaShark.HeaderAttribute;


namespace SOSXR.plet
{
    [CreateAssetMenu(fileName = "PletSceneSettings", menuName = "SOSXR/plet/PletSceneSettings", order = 1)]
    public class PletSceneSettings : ScriptableObject
    {
        public Palette Palette;

        [Header("Skybox Settings")]
        public bool ApplySkybox = false;
        public Material SkyboxMaterial;
        public HueType SkyboxSkyHueType;
        public int SkyboxSkySaturation = 10;
        public int SkyboxSkyValue = 10;
        public Color SkyboxSkyColor;

        public HueType SkyboxHorizonHueType;
        public int SkyboxHorizonSaturation = 10;
        public int SkyboxHorizonValue = 10;
        public Color SkyboxHorizonColor;

        public HueType SkyboxGroundHueType;
        public int SkyboxGroundSaturation = 10;
        public int SkyboxGroundValue = 10;
        public Color SkyboxGroundColor;

        [Header("Ambient Light Settings")]
        public bool ApplyAmbientLight = false;

        public HueType AmbientLightHueType;
        public int AmbientLightSaturation = 10;
        public int AmbientLightValue = 10;
        public Color AmbientLightColor;

        public HueType AmbientSkyLightHueType;
        public int AmbientSkyLightSaturation = 10;
        public int AmbientSkyLightValue = 10;
        public Color AmbientSkyLightColor;

        public HueType AmbientEquatorLightHueType;
        public int AmbientEquatorLightSaturation = 10;
        public int AmbientEquatorLightValue = 10;
        public Color AmbientEquatorLightColor;

        public HueType AmbientGroundLightHueType;
        public int AmbientGroundLightSaturation = 10;
        public int AmbientGroundLightValue = 10;
        public Color AmbientGroundLightColor;

        [Header("Shadow Settings")]
        public bool ApplyRealtimeShadows = false;
        public HueType RealtimeShadowHueType;
        public int RealtimeShadowSaturation = 10;
        public int RealtimeShadowValue = 10;
        public Color RealtimeShadowColor;

        [Header("Fog Settings")]
        public bool ApplyFog = false;
        public HueType FogHueType;
        public int FogSaturation = 10;
        public int FogValue = 10;
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
                if (Palette == null)
                {
                    Debug.LogWarning("Palette is null", this);
                }

                return;
            }

            SetAllSkyboxAndLights();

            if (_previousPalette != Palette)
            {
                _allColorProviders = FindObjectsByType<ColorProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                foreach (var colorProvider in _allColorProviders)
                {
                    colorProvider.Init();
                }

                _previousPalette = Palette;
            }
        }


        public bool IsActiveSceneSettings()
        {
            return PletHelpers.GetPletSceneSettings() == this;
        }


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


        public void SetRealtimeShadowColor()
        {
            if (!IsActiveSceneSettings() || !ApplyRealtimeShadows)
            {
                return;
            }

            RealtimeShadowColor = ApplyColor(RealtimeShadowHueType, RealtimeShadowSaturation, RealtimeShadowValue);
            RenderSettings.subtractiveShadowColor = RealtimeShadowColor;
        }


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

            // Editor delay call
            EditorApplication.delayCall += () => { CreateSceneSkyboxMaterial(sceneMatName); };
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