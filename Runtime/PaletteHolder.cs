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
        [SerializeField] private float m_skyColorValue = 1f;
        [SerializeField] private float m_skyColorSaturation = 1f;
        [SerializeField] private Color m_skyColor;
        [SerializeField] private ColorType m_horizonColorType;
        [SerializeField] private float m_horizonColorValue = 1f;
        [SerializeField] private float m_horizonColorSaturation = 1f;
        [SerializeField] private Color m_horizonColor;
        [SerializeField] private ColorType m_groundColorType;
        [SerializeField] private float m_groundColorValue = 1f;
        [SerializeField] private float m_groundColorSaturation = 1f;
        [SerializeField] private Color m_groundColor;

        [SerializeField] private bool m_applyAmbientLight = true;

        [SerializeField] private ColorType m_ambientLightType;
        [SerializeField] private float m_ambientLightValue = 1f;
        [SerializeField] private float m_ambientLightSaturation = 1f;
        [SerializeField] private Color m_ambientLightColor;

        [SerializeField] private ColorType m_ambientSkyLightType;
        [SerializeField] private float m_ambientSkyLightValue = 1f;
        [SerializeField] private float m_ambientSkyLightSaturation = 1f;
        [SerializeField] private Color m_ambientSkyLightColor;
        [SerializeField] private ColorType m_ambientEquatorLightType;
        [SerializeField] private float m_ambientEquatorLightValue = 1f;
        [SerializeField] private float m_ambientEquatorLightSaturation = 1f;
        [SerializeField] private Color m_ambientEquatorLightColor;
        [SerializeField] private ColorType m_ambientGroundLightType;
        [SerializeField] private float m_ambientGroundLightValue = 1f;
        [SerializeField] private float m_ambientGroundLightSaturation = 1f;
        [SerializeField] private Color m_ambientGroundLightColor;

        [SerializeField] private bool m_applyRealtimeShadows;
        [SerializeField] private Color m_shadowColor;
        [SerializeField] private ColorType m_shadowColorType;
        [SerializeField] private float m_shadowColorValue = 1f;
        [SerializeField] private float m_shadowColorSaturation = 1f;

        [SerializeField] private bool m_applyFog;
        [SerializeField] private Color m_fogColor;
        [SerializeField] private ColorType m_fogColorType;
        [SerializeField] private float m_fogColorValue = 1f;
        [SerializeField] private float m_fogColorSaturation = 1f;

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

            m_skyColorValue = 1f;
            m_skyColorSaturation = 1f;
            m_horizonColorValue = 1f;
            m_horizonColorSaturation = 1f;
            m_groundColorValue = 1f;
            m_groundColorSaturation = 1f;
            m_ambientLightValue = 1f;
            m_ambientLightSaturation = 1f;
            m_ambientSkyLightValue = 1f;
            m_ambientSkyLightSaturation = 1f;
            m_ambientEquatorLightValue = 1f;
            m_ambientEquatorLightSaturation = 1f;
            m_ambientGroundLightValue = 1f;
            m_ambientGroundLightSaturation = 1f;

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

            m_ambientLightColor = ApplyColor(m_ambientLightType, m_ambientLightValue, m_ambientLightSaturation);
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

            m_ambientSkyLightColor = ApplyColor(m_ambientSkyLightType, m_ambientSkyLightValue, m_ambientSkyLightSaturation);
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

            m_ambientEquatorLightColor = ApplyColor(m_ambientEquatorLightType, m_ambientEquatorLightValue, m_ambientEquatorLightSaturation);
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

            m_ambientGroundLightColor = ApplyColor(m_ambientGroundLightType, m_ambientGroundLightValue, m_ambientGroundLightSaturation);
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

            m_shadowColor = ApplyColor(m_shadowColorType, m_shadowColorValue, m_shadowColorSaturation);
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

            m_fogColor = ApplyColor(m_fogColorType, m_fogColorValue, m_fogColorSaturation);
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

            m_skyColor = ApplyColor(m_skyColorType, m_skyColorValue, m_skyColorSaturation);
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

            m_horizonColor = ApplyColor(m_horizonColorType, m_horizonColorValue, m_horizonColorSaturation);
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

            m_groundColor = ApplyColor(m_groundColorType, m_groundColorValue, m_groundColorSaturation);
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


        public Color ApplyColor(ColorType type, float valueAdjust, float saturation, float alpha = 1f)
        {
            var baseColor = GetColor(type);

            Color.RGBToHSV(baseColor, out var h, out var s, out var v);
            var newColor = Color.HSVToRGB(h, Mathf.Clamp(s * saturation, SatClamp.x, SatClamp.y), Mathf.Clamp(v * valueAdjust, ValueClamp.x, ValueClamp.y));
            newColor.a = alpha;

            return newColor;
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