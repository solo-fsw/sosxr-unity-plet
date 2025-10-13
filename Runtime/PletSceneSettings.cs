using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SOSXR.plet
{
    [CreateAssetMenu(fileName = "PletSceneSettings", menuName = "SOSXR/plet/PletSceneSettings", order = 1)]
    public class PletSceneSettings : ScriptableObject
    {
        public Palette Palette;
        public Palette PreviousPalette;

        public bool ApplySkybox = true;
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

        public bool ApplyAmbientLight = true;

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

        public bool ApplyRealtimeShadows = true;

        public HueType RealtimeShadowHueType;
        public int RealtimeShadowSaturation = 10;
        public int RealtimeShadowValue = 10;
        public Color RealtimeShadowColor;

        public bool ApplyFog = true;

        public HueType FogHueType;
        public int FogSaturation = 10;
        public int FogValue = 10;
        public Color FogColor;

        private readonly int _skyColorShaderId = Shader.PropertyToID("_SkyColor");
        private readonly int _horizonColorShaderId = Shader.PropertyToID("_HorizonColor");
        private readonly int _groundColorShaderId = Shader.PropertyToID("_GroundColor");

        public Action OnPaletteChanged;


        private void OnValidate()
        {
            Init();
        }


        private void Reset()
        {
            Init();
        }


        private void Init()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (Palette == null)
            {
                Debug.LogWarning("Palette is null");

                return;
            }

            Palette.SceneSettings = this;

            if (ApplySkybox && SkyboxMaterial == null)
            {
                SkyboxMaterial = Resources.Load<Material>("TriColorSkybox/plet_skybox"); // It's in the Resources folder in the Samples of this package.
            }

            PreviousPalette = Palette;

            SetAllSkyboxAndLights();

            OnPaletteChanged?.Invoke();
        }


        public static bool UseThisPaletteSceneSettings()
        {
            return PletHelpers.GetPletSceneSettings() != null;
        }


        public void SetAllSkyboxAndLights()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            SetSkyboxMaterial();
            SetSkyboxSkyColor();
            SetSkyboxHorizonColor();
            SetSkyboxGroundColor();
            SetAmbientLightColor();
            SetAmbientSkyTriLightColor();
            SetAmbientEquatorTriLightColor();
            SetAmbientGroundTriLightColor();
            SetRealtimeShadowColor();
            SetFogColor();

#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }


        [ContextMenu(nameof(GetPaletteSaturationAndValue))]
        public void GetPaletteSaturationAndValue()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            GetSkyboxSVFromPalette();

            GetAmbientLightSVFromPalette();

            GetRealtimeShadowSVFromPalette();

            GetFogSVFromPalette();
        }


        public void GetSkyboxSVFromPalette()
        {
            var skySV = GetColorSV(SkyboxSkyHueType);
            SkyboxSkySaturation = skySV.x;
            SkyboxSkyValue = skySV.y;

            var horizonSV = GetColorSV(SkyboxHorizonHueType);
            SkyboxHorizonSaturation = horizonSV.x;
            SkyboxHorizonValue = horizonSV.y;

            var groundSV = GetColorSV(SkyboxGroundHueType);
            SkyboxGroundSaturation = groundSV.x;
            SkyboxGroundValue = groundSV.y;

            SetAllSkyboxAndLights();
        }


        public void GetAmbientLightSVFromPalette()
        {
            var ambientSV = GetColorSV(AmbientLightHueType);
            AmbientLightSaturation = ambientSV.x;
            AmbientLightValue = ambientSV.y;

            var ambientSkySV = GetColorSV(AmbientSkyLightHueType);
            AmbientSkyLightSaturation = ambientSkySV.x;
            AmbientSkyLightValue = ambientSkySV.y;

            var ambientEquatorSV = GetColorSV(AmbientEquatorLightHueType);
            AmbientEquatorLightSaturation = ambientEquatorSV.x;
            AmbientEquatorLightValue = ambientEquatorSV.y;

            var ambientGroundSV = GetColorSV(AmbientGroundLightHueType);
            AmbientGroundLightSaturation = ambientGroundSV.x;
            AmbientGroundLightValue = ambientGroundSV.y;

            SetAllSkyboxAndLights();
        }


        public void GetRealtimeShadowSVFromPalette()
        {
            var shadowSV = GetColorSV(RealtimeShadowHueType);
            RealtimeShadowSaturation = shadowSV.x;
            RealtimeShadowValue = shadowSV.y;

            SetAllSkyboxAndLights();
        }


        public void GetFogSVFromPalette()
        {
            var fogSV = GetColorSV(FogHueType);
            FogSaturation = fogSV.x;
            FogValue = fogSV.y;

            SetAllSkyboxAndLights();
        }


        public void SetAmbientLightColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Flat)
            {
                return;
            }

            AmbientLightColor = ApplyColor(AmbientLightHueType, AmbientLightSaturation, AmbientLightValue);
            RenderSettings.ambientLight = AmbientLightColor;
        }


        public void SetAmbientSkyTriLightColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientSkyLightColor = ApplyColor(AmbientSkyLightHueType, AmbientSkyLightSaturation, AmbientSkyLightValue);
            RenderSettings.ambientSkyColor = AmbientSkyLightColor;
        }


        public void SetAmbientEquatorTriLightColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientEquatorLightColor = ApplyColor(AmbientEquatorLightHueType, AmbientEquatorLightSaturation, AmbientEquatorLightValue);
            RenderSettings.ambientEquatorColor = AmbientEquatorLightColor;
        }


        public void SetAmbientGroundTriLightColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyAmbientLight || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            AmbientGroundLightColor = ApplyColor(AmbientGroundLightHueType, AmbientGroundLightSaturation, AmbientGroundLightValue);
            RenderSettings.ambientGroundColor = AmbientGroundLightColor;
        }


        public void SetRealtimeShadowColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyRealtimeShadows)
            {
                return;
            }

            RealtimeShadowColor = ApplyColor(RealtimeShadowHueType, RealtimeShadowSaturation, RealtimeShadowValue);
            RenderSettings.subtractiveShadowColor = RealtimeShadowColor;
        }


        public void SetFogColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplyFog)
            {
                return;
            }

            FogColor = ApplyColor(FogHueType, FogSaturation, FogValue);
            RenderSettings.fogColor = FogColor;
        }


        public void SetSkyboxSkyColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplySkybox || SkyboxMaterial == null || Palette == null)
            {
                return;
            }

            if (_skyColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            SkyboxSkyColor = ApplyColor(SkyboxSkyHueType, SkyboxSkySaturation, SkyboxSkyValue);
            SkyboxMaterial.SetColor(_skyColorShaderId, SkyboxSkyColor);
        }


        public void SetSkyboxHorizonColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplySkybox || SkyboxMaterial == null || Palette == null)
            {
                return;
            }

            if (_horizonColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            SkyboxHorizonColor = ApplyColor(SkyboxHorizonHueType, SkyboxHorizonSaturation, SkyboxHorizonValue);
            SkyboxMaterial.SetColor(_horizonColorShaderId, SkyboxHorizonColor);
        }


        public void SetSkyboxGroundColor()
        {
            if (!UseThisPaletteSceneSettings())
            {
                return;
            }

            if (!ApplySkybox || SkyboxMaterial == null || Palette == null)
            {
                return;
            }

            if (_horizonColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            SkyboxGroundColor = ApplyColor(SkyboxGroundHueType, SkyboxGroundSaturation, SkyboxGroundValue);
            SkyboxMaterial.SetColor(_groundColorShaderId, SkyboxGroundColor);
        }


        public void SetSkyboxMaterial()
        {
            if (!UseThisPaletteSceneSettings() || !ApplySkybox || SkyboxMaterial == null)
            {
                return;
            }

            RenderSettings.skybox = SkyboxMaterial;
        }


        public Color ApplyColor(HueType type, int saturation, int value, float alpha = 1f)
        {
            var baseColor = GetColorFromPalette(type);

            Color.RGBToHSV(baseColor, out var h, out var s, out var v);

            // Map the Ranges.cs DisplayRange to a correctly clamped HSV value (0-1)
            s = Mathf.Lerp(Saturation.Clamp.x, Saturation.Clamp.y, (saturation - 1) / ((float)Saturation.DisplayRange.y - 1));
            v = Mathf.Lerp(Value.Clamp.x, Value.Clamp.y, (value - 1) / ((float)Value.DisplayRange.y - 1));

            var newColor = Color.HSVToRGB(h, s, v);
            newColor.a = alpha;

            return newColor;
        }


        public Vector2Int GetColorSV(HueType type)
        {
            var baseColor = GetColorFromPalette(type);
            Color.RGBToHSV(baseColor, out var h, out var s, out var v);

            // Map the Ranges.cs DisplayRange to a correctly clamped HSV value (0-1)
            var saturation = Mathf.RoundToInt((s - Saturation.Clamp.x) / (Saturation.Clamp.y - Saturation.Clamp.x) * ((float)Saturation.DisplayRange.y - 1)) + 1;
            var value = Mathf.RoundToInt((v - Value.Clamp.x) / (Value.Clamp.y - Value.Clamp.x) * ((float)Value.DisplayRange.y - 1)) + 1;

            return new Vector2Int(saturation, value);
        }


        private Color GetColorFromPalette(HueType type)
        {
            if (Palette == null)
            {
                return Color.black;
            }

            var baseColor = type switch
            {
                HueType.Base => Palette.Base,
                HueType.Tone => Palette.Tone,
                HueType.Accent => Palette.Accent,
                _ => Color.white
            };

            return baseColor;
        }
    }
}