using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SOSXR.plet
{
    /// <summary>
    ///     MonoBehaviour that applies palette-driven colors to a supported component on the same GameObject.
    ///     Supports <see cref="Renderer"/> (via MaterialPropertyBlock), <see cref="Light"/>,
    ///     <see cref="Camera"/>, <see cref="SpriteRenderer"/>, <see cref="UnityEngine.UI.Image"/>,
    ///     <see cref="TMPro.TMP_Text"/>, <see cref="UnityEngine.UI.Text"/>,
    ///     <see cref="UnityEngine.UI.Selectable"/>, and <see cref="ParticleSystem"/>.
    ///     Each material slot gets its own <see cref="ColorSettings"/> entry with an independently
    ///     adjustable hue type, saturation (1–19 scale), value/brightness (1–19 scale), and alpha.
    /// </summary>
    [ExecuteInEditMode]
    public class ColorProvider : MonoBehaviour
    {
        /// <summary>One entry per material slot (or per color channel for UI <see cref="UnityEngine.UI.Selectable"/>s).</summary>
        public List<ColorSettings> ColorSettings = new(1);

        /// <summary>Tracks whether this provider has performed its one-time palette-derived initialisation.</summary>
        [SerializeField] private bool init;
        /// <summary>Cached active scene settings used to resolve palette colors and SV mapping.</summary>
        [HideInInspector] [SerializeField] private PletSceneSettings m_pletSceneSettings;

        private static readonly Dictionary<Type, Action<Component, ColorProvider>> ColorAppliers = new()
        {
            {typeof(SpriteRenderer), (c, cp) => ((SpriteRenderer) c).color = cp.ColorSettings[0].FinalColor},
            {typeof(Renderer), ApplyColorToRenderer},
            {typeof(Selectable), ApplyColorToSelectable},
            {typeof(Image), (c, cp) => ((Image) c).color = cp.ColorSettings[0].FinalColor},
            {typeof(TMP_Text), (c, cp) => ((TMP_Text) c).color = cp.ColorSettings[0].FinalColor},
            {typeof(Text), (c, cp) => ((Text) c).color = cp.ColorSettings[0].FinalColor},
            {
                typeof(Light), (c, cp) =>
                {
                    ((Light) c).color = cp.ColorSettings[0].FinalColor;
                    cp.ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(Camera), (c, cp) =>
                {
                    var cam = (Camera) c;
                    cam.clearFlags = CameraClearFlags.Color;
                    cam.backgroundColor = cp.ColorSettings[0].FinalColor;
                    cp.ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(ParticleSystem), (c, cp) =>
                {
                    var ps = (ParticleSystem) c;
                    var main = ps.main;
                    main.startColor = cp.ColorSettings[0].FinalColor;
                }
            }
        };

        private static readonly int _colorShaderId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _mpb;
        private Component _component;
        private Action _applyColorAction;


        private static void ApplyColorToSelectable(Component c, ColorProvider colorProvider)
        {
            var selectable = (Selectable) c;
            var colors = selectable.colors;

            EnsureCorrectLength(colorProvider, 5);

            colorProvider.ColorSettings[0].Name = nameof(colors.normalColor);
            colors.normalColor = colorProvider.ColorSettings[0].FinalColor;

            colorProvider.ColorSettings[1].Name = nameof(colors.highlightedColor);
            colors.highlightedColor = colorProvider.ColorSettings[1].FinalColor;

            colorProvider.ColorSettings[2].Name = nameof(colors.pressedColor);
            colors.pressedColor = colorProvider.ColorSettings[2].FinalColor;

            colorProvider.ColorSettings[3].Name = nameof(colors.selectedColor);
            colors.selectedColor = colorProvider.ColorSettings[3].FinalColor;

            colorProvider.ColorSettings[4].Name = nameof(colors.disabledColor);
            colors.disabledColor = colorProvider.ColorSettings[4].FinalColor;

            selectable.colors = colors;
        }


        private static void ApplyColorToRenderer(Component c, ColorProvider colorProvider)
        {
            var renderer = (Renderer) c;

            if (renderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning($"{renderer.GetType().Name} has no materials.", renderer);

                return;
            }

            colorProvider._mpb ??= new MaterialPropertyBlock();

            EnsureCorrectLength(colorProvider, renderer.sharedMaterials.Length);

            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                var material = renderer.sharedMaterials[i];

                if (material == null)
                {
                    continue;
                }

                colorProvider.ColorSettings[i].Name = material.name;
                renderer.GetPropertyBlock(colorProvider._mpb, i);
                colorProvider._mpb.SetColor(_colorShaderId, colorProvider.ColorSettings[i].FinalColor);
                renderer.SetPropertyBlock(colorProvider._mpb, i);
                colorProvider.ColorSettings[i].ShowAlpha = UsesAlpha(material);
            }
        }


        private static void EnsureCorrectLength(ColorProvider colorProvider, int lengthNeeded)
        {
            while (colorProvider.ColorSettings.Count < lengthNeeded)
            {
                colorProvider.ColorSettings.Add(new ColorSettings());
            }

            if (colorProvider.ColorSettings.Count > lengthNeeded)
            {
                colorProvider.ColorSettings.RemoveRange(lengthNeeded, colorProvider.ColorSettings.Count - lengthNeeded);
            }
        }


        private static bool UsesAlpha(Material material)
        {
            return material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") ||
                   material.IsKeywordEnabled("_ALPHATEST_ON");
        }


        private void OnValidate()
        {
            Init();
        }


        /// <summary>
        ///     Initialises this provider: fetches the active <see cref="PletSceneSettings"/>, ensures at least
        ///     one <see cref="ColorSettings"/> entry exists, and on first run reads saturation/value from the
        ///     palette. On subsequent calls it applies the current color settings to the target component.
        /// </summary>
        [Button]
        public void Init()
        {
            if (!enabled)
            {
                return;
            }

            GetSceneSettings();

            if (ColorSettings.Count == 0)
            {
                ColorSettings.Add(new ColorSettings());
            }

            if (!init)
            {
                GetPaletteSaturationAndValue();
                init = true;

                return;
            }

            ApplyColorAction();
        }


        /// <summary>
        ///     Reads saturation and value from the active palette for each <see cref="ColorSettings"/> entry
        ///     and distributes hue types across multiple material slots on first initialisation.
        ///     Call this whenever the palette changes or the component is first set up.
        /// </summary>
        [Button]
        public void GetPaletteSaturationAndValue()
        {
            if (!GetSceneSettings())
            {
                return;
            }

            // Ensure correct settings count for renderer materials
            if (TryGetComponent<Renderer>(out var rend) && rend.sharedMaterials.Length > 0)
            {
                EnsureCorrectLength(this, rend.sharedMaterials.Length);

                for (var i = 0; i < rend.sharedMaterials.Length; i++)
                {
                    if (rend.sharedMaterials[i] != null && i < ColorSettings.Count)
                    {
                        ColorSettings[i].Name = rend.sharedMaterials[i].name;
                    }
                }
            }

            // Initialize all color settings
            for (var i = 0; i < ColorSettings.Count; i++)
            {
                // Distribute different hue types across materials on first init
                if (!init && i > 0)
                {
                    var hueTypeCount = Enum.GetValues(typeof(HueType)).Length;
                    ColorSettings[i].HueType = (HueType) (((int) ColorSettings[0].HueType + i) % hueTypeCount);
                }

                var satVal = m_pletSceneSettings.GetColorSV(ColorSettings[i].HueType);
                ColorSettings[i].Saturation = satVal.x;
                ColorSettings[i].Value = satVal.y;
                ColorSettings[i].Alpha = 1f;

                ColorSettings[i].FinalColor = m_pletSceneSettings.ApplyColor(
                    ColorSettings[i].HueType,
                    ColorSettings[i].Saturation,
                    ColorSettings[i].Value,
                    ColorSettings[i].Alpha
                );
            }

            ApplyColorAction();
        }


        private bool GetSceneSettings()
        {
            if (m_pletSceneSettings != null)
            {
                return true;
            }

            m_pletSceneSettings = PletHelpers.GetPletSceneSettings();

            return m_pletSceneSettings != null;
        }


        private void CacheComponent()
        {
            _component = null;
            _applyColorAction = null;

            foreach (var kvp in ColorAppliers)
            {
                if (!TryGetComponent(kvp.Key, out var component))
                {
                    continue;
                }

                _component = component;
                var applier = kvp.Value;
                _applyColorAction = () => applier(component, this);

                break;
            }
        }


        /// <summary>
        ///     Resolves the appropriate color-application delegate for the component type and invokes it,
        ///     updating all target color channels from the active palette.
        ///     Safe to call every frame or on demand.
        /// </summary>
        [Button]
        public void ApplyColorAction()
        {
            if (!enabled || !GetSceneSettings())
            {
                return;
            }

            if (_component == null || _applyColorAction == null)
            {
                CacheComponent();
            }

            if (_applyColorAction == null)
            {
                Debug.LogWarning("Component type not supported or not found.", this);

                return;
            }

            // Update final colors from palette
            foreach (var setting in ColorSettings)
            {
                setting.FinalColor = m_pletSceneSettings.ApplyColor(
                    setting.HueType,
                    setting.Saturation,
                    setting.Value,
                    setting.Alpha
                );
            }

            _applyColorAction.Invoke();
        }
    }
}
