using System;
using System.Collections.Generic;
using System.Linq;
using SOSXR.SeaShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SOSXR.plet
{
    /// <summary>
    ///     Add this component to a GameObject to apply color settings to its components.
    /// </summary>
    [ExecuteInEditMode]
    public class ColorProvider : MonoBehaviour
    {
        public List<ColorSettings> ColorSettings = new(1);

        [SerializeField] private bool init;
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

        private readonly int _colorShaderId = Shader.PropertyToID("_BaseColor");
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
                colorProvider._mpb.SetColor(colorProvider._colorShaderId, colorProvider.ColorSettings[i].FinalColor);
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

            /*// Try to find a supported component type
            var componentTypes = new[]
            {
                typeof(Light), typeof(SpriteRenderer), typeof(Renderer), typeof(Camera),
                typeof(ParticleSystem), typeof(Selectable), typeof(Image), typeof(TMP_Text)
            };*/

            foreach (var type in ColorAppliers.Keys)
            {
                if (!TryGetComponent(type, out var component))
                {
                    continue;
                }

                _component = component;

                foreach (var entry in ColorAppliers.Where(entry => component.GetType() == entry.Key || component.GetType().IsSubclassOf(entry.Key)))
                {
                    _applyColorAction = () => entry.Value(component, this);

                    break;
                }
            }
        }


        [Button]
        public void ApplyColorAction()
        {
            if (!enabled || !GetSceneSettings())
            {
                return;
            }

            CacheComponent();

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