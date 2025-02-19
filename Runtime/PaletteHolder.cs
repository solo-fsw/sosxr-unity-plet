using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


namespace SOSXR.Plet
{
    [CreateAssetMenu(fileName = "PaletteHolder", menuName = "SOSXR/Plet/PaletteHolder", order = 1)]
    public class PaletteHolder : ScriptableObject
    {
        [SerializeField] private Palette m_palette;
        [SerializeField] private Palette m_previousPalette;

        [SerializeField] private bool m_applySkybox = true;

        [SerializeField] private Material m_skyboxMaterial;

        [SerializeField] private ColorType m_skyColorType;
        [SerializeField] private int m_skyColorValue = 1;
        [SerializeField] private int m_skyColorSaturation = 1;
        [SerializeField] private Color m_skyColor;
        [SerializeField] private ColorType m_horizonColorType;
        [SerializeField] private int m_horizonColorValue = 1;
        [SerializeField] private int m_horizonColorSaturation = 1;
        [SerializeField] private Color m_horizonColor;
        [SerializeField] private ColorType m_groundColorType;
        [SerializeField] private int m_groundColorValue = 1;
        [SerializeField] private int m_groundColorSaturation = 1;
        [SerializeField] private Color m_groundColor;

        [SerializeField] private bool m_applyAmbientLight = true;

        [SerializeField] private ColorType m_ambientLightType;
        [SerializeField] private int m_ambientLightValue = 1;
        [SerializeField] private int m_ambientLightSaturation = 1;
        [SerializeField] private Color m_ambientLightColor;

        [SerializeField] private ColorType m_ambientSkyLightType;
        [SerializeField] private int m_ambientSkyLightValue = 1;
        [SerializeField] private int m_ambientSkyLightSaturation = 1;
        [SerializeField] private Color m_ambientSkyLightColor;
        [SerializeField] private ColorType m_ambientEquatorLightType;
        [SerializeField] private int m_ambientEquatorLightValue = 1;
        [SerializeField] private int m_ambientEquatorLightSaturation = 1;
        [SerializeField] private Color m_ambientEquatorLightColor;
        [SerializeField] private ColorType m_ambientGroundLightType;
        [SerializeField] private int m_ambientGroundLightValue = 1;
        [SerializeField] private int m_ambientGroundLightSaturation = 1;
        [SerializeField] private Color m_ambientGroundLightColor;

        [SerializeField] private bool m_applyRealtimeShadows;
        [SerializeField] private Color m_shadowColor;
        [SerializeField] private ColorType m_shadowColorType;
        [SerializeField] private int m_shadowColorValue = 1;
        [SerializeField] private int m_shadowColorSaturation = 1;

        [SerializeField] private bool m_applyFog;
        [SerializeField] private Color m_fogColor;
        [SerializeField] private ColorType m_fogColorType;
        [SerializeField] private int m_fogColorValue = 1;
        [SerializeField] private int m_fogColorSaturation = 1;

        private readonly int _skyColorShaderId = Shader.PropertyToID("_SkyColor");
        private readonly int _horizonColorShaderId = Shader.PropertyToID("_HorizonColor");
        private readonly int _groundColorShaderId = Shader.PropertyToID("_GroundColor");

        private readonly Vector2 ValueClamp = new(0.0075f, 1.00f);
        private readonly Vector2 SatClamp = new(0.0075f, 2.25f);

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
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_palette == null)
            {
                Debug.LogWarning("Palette is null");

                return;
            }

            m_palette.PaletteHolder = this;

            if (m_applySkybox && m_skyboxMaterial == null)
            {
                m_skyboxMaterial = Resources.Load<Material>("TriColorSkybox/plet_skybox"); // It's in the Samples of this package.
            }

            m_previousPalette = m_palette;

            SetAllSkyboxAndLights();

            OnPaletteChanged?.Invoke();
        }


        public bool UseThisPaletteHolder()
        {
            var paletteHolders = Resources.FindObjectsOfTypeAll<PaletteHolder>();

            if (paletteHolders.Length == 1)
            {
                return true;
            }

            if (paletteHolders.Length == 0)
            {
                Debug.LogWarning("No PaletteHolder found in the Resources folder.");

                return false;
            }

            return name == SceneManager.GetActiveScene().name;
        }


        public void SetAllSkyboxAndLights()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            SetSkyboxMaterial();
            SetSkyColor();
            SetHorizonColor();
            SetGroundColor();
            SetAmbientLight();
            SetAmbientSkyLight();
            SetAmbientEquatorLight();
            SetAmbientGroundLight();
            SetRealtimeShadowColor();
            SetFogColor();
        }


        [ContextMenu(nameof(GetBaseValues))]
        private void GetBaseValues()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            var skySV = GetColorSV(m_skyColorType);
            m_skyColorSaturation = skySV.x;
            m_skyColorValue = skySV.y;
            
            var horizonSV = GetColorSV(m_horizonColorType);
            m_horizonColorSaturation = horizonSV.x;
            m_horizonColorValue = horizonSV.y;
            
            var groundSV = GetColorSV(m_groundColorType);
            m_groundColorSaturation = groundSV.x;
            m_groundColorValue = groundSV.y;
            
            var ambientSV = GetColorSV(m_ambientLightType);
            m_ambientLightSaturation = ambientSV.x;
            m_ambientLightValue = ambientSV.y;
            
            var ambientSkySV = GetColorSV(m_ambientSkyLightType);
            m_ambientSkyLightSaturation = ambientSkySV.x;
            m_ambientSkyLightValue = ambientSkySV.y;
            
            var ambientEquatorSV = GetColorSV(m_ambientEquatorLightType);
            m_ambientEquatorLightSaturation = ambientEquatorSV.x;
            m_ambientEquatorLightValue = ambientEquatorSV.y;    
            
            var ambientGroundSV = GetColorSV(m_ambientGroundLightType);
            m_ambientGroundLightSaturation = ambientGroundSV.x;
            m_ambientGroundLightValue = ambientGroundSV.y;
            
            var shadowSV = GetColorSV(m_shadowColorType);
            m_shadowColorSaturation = shadowSV.x;
            m_shadowColorValue = shadowSV.y;
            
            var fogSV = GetColorSV(m_fogColorType);
            m_fogColorSaturation = fogSV.x;
            m_fogColorValue = fogSV.y;

            SetAllSkyboxAndLights();
        }


        public void SetAmbientLight()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyAmbientLight == false || RenderSettings.ambientMode != AmbientMode.Flat)
            {
                return;
            }

            m_ambientLightColor = ApplyColor(m_ambientLightType, m_ambientLightSaturation, m_ambientLightValue);
            RenderSettings.ambientLight = m_ambientLightColor;
        }


        public void SetAmbientSkyLight()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyAmbientLight == false || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            m_ambientSkyLightColor = ApplyColor(m_ambientSkyLightType, m_ambientSkyLightSaturation, m_ambientSkyLightValue);
            RenderSettings.ambientSkyColor = m_ambientSkyLightColor;
        }


        public void SetAmbientEquatorLight()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyAmbientLight == false || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            m_ambientEquatorLightColor = ApplyColor(m_ambientEquatorLightType, m_ambientEquatorLightSaturation, m_ambientEquatorLightValue);
            RenderSettings.ambientEquatorColor = m_ambientEquatorLightColor;
        }


        public void SetAmbientGroundLight()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyAmbientLight == false || RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                return;
            }

            m_ambientGroundLightColor = ApplyColor(m_ambientGroundLightType, m_ambientGroundLightSaturation, m_ambientGroundLightValue);
            RenderSettings.ambientGroundColor = m_ambientGroundLightColor;
        }


        public void SetRealtimeShadowColor()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyRealtimeShadows == false)
            {
                return;
            }

            m_shadowColor = ApplyColor(m_shadowColorType, m_shadowColorSaturation, m_shadowColorValue);
            RenderSettings.subtractiveShadowColor = m_shadowColor;
        }


        public void SetFogColor()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applyFog == false)
            {
                return;
            }

            m_fogColor = ApplyColor(m_fogColorType, m_fogColorSaturation, m_fogColorValue);
            RenderSettings.fogColor = m_fogColor;
        }


        public void SetSkyColor()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applySkybox == false || m_skyboxMaterial == null || m_palette == null)
            {
                return;
            }

            if (_skyColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            m_skyColor = ApplyColor(m_skyColorType, m_skyColorSaturation, m_skyColorValue);
            m_skyboxMaterial.SetColor(_skyColorShaderId, m_skyColor);
        }


        public void SetHorizonColor()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applySkybox == false || m_skyboxMaterial == null || m_palette == null)
            {
                return;
            }

            if (_horizonColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            m_horizonColor = ApplyColor(m_horizonColorType, m_horizonColorSaturation, m_horizonColorValue);
            m_skyboxMaterial.SetColor(_horizonColorShaderId, m_horizonColor);
        }


        public void SetGroundColor()
        {
            if (!UseThisPaletteHolder())
            {
                return;
            }

            if (m_applySkybox == false || m_skyboxMaterial == null || m_palette == null)
            {
                return;
            }

            if (_horizonColorShaderId == 0)
            {
                Debug.LogWarningFormat(this, "Skybox does not have the correct properties. Does it have the correct shader ({0})?", "SkyboxShader");

                return;
            }

            m_groundColor = ApplyColor(m_groundColorType, m_groundColorSaturation, m_groundColorValue);
            m_skyboxMaterial.SetColor(_groundColorShaderId, m_groundColor);
        }


        public Color GetDominantColor()
        {
            return m_palette == null ? Color.black : m_palette.Base;
        }


        public Color GetSecondaryColor()
        {
            return m_palette == null ? Color.black : m_palette.Tone;
        }


        public Color GetAccentColor()
        {
            return m_palette == null ? Color.black : m_palette.Accent;
        }


        public void SetSkyboxMaterial()
        {
            RenderSettings.skybox = m_skyboxMaterial;
        }


        public Color ApplyColor(ColorType type, int saturation, int value, float alpha = 1f)
        {
            var baseColor = GetColor(type);

            Color.RGBToHSV(baseColor, out var h, out var s, out var v);

            // Map the 1-19 scale to a reasonable HSV range
            s = Mathf.Lerp(SatClamp.x, SatClamp.y, (saturation - 1) / 18f);
            v = Mathf.Lerp(ValueClamp.x, ValueClamp.y, (value - 1) / 18f);

            var newColor = Color.HSVToRGB(h, s, v);
            newColor.a = alpha;

            return newColor;
        }


        public Vector2Int GetColorSV(ColorType type)
        {
            var baseColor = GetColor(type);
            Color.RGBToHSV(baseColor, out var h, out var s, out var v);

            var saturation = Mathf.RoundToInt((s - SatClamp.x) / (SatClamp.y - SatClamp.x) * 18f) + 1;
            var value = Mathf.RoundToInt((v - ValueClamp.x) / (ValueClamp.y - ValueClamp.x) * 18f) + 1;

            return new Vector2Int( saturation, value);
        }
        

        public Color GetColor(ColorType type)
        {
            var baseColor = type switch
                            {
                                ColorType.Base => GetDominantColor(),
                                ColorType.Tone => GetSecondaryColor(),
                                ColorType.Accent => GetAccentColor(),
                                _ => Color.white
                            };

            return baseColor;
        }
    }
}