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
    ///     <see cref="Camera"/>, <see cref="SpriteRenderer"/>, <see cref="Image"/>,
    ///     <see cref="TMP_Text"/>, <see cref="Text"/>,
    ///     <see cref="Selectable"/>, and <see cref="ParticleSystem"/>.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class ColorProvider : MonoBehaviour
    {
        public List<ColorSettings> ColorSettings = new(1);

        [SerializeField] private bool _init;
        [HideInInspector][SerializeField] private PletSceneSettings _pletSceneSettings;

        private static readonly Dictionary<Type, Action<Component, ColorProvider>> s_colorAppliers = new()
        {
            { typeof(SpriteRenderer), static (c, cp) => ((SpriteRenderer)c).color = cp.ColorSettings[0].FinalColor },
            { typeof(Renderer), ApplyColorToRenderer },
            { typeof(Selectable), ApplyColorToSelectable },
            { typeof(Image), static (c, cp) => ((Image)c).color = cp.ColorSettings[0].FinalColor },
            { typeof(TMP_Text), static (c, cp) => ((TMP_Text)c).color = cp.ColorSettings[0].FinalColor },
            { typeof(Text), static (c, cp) => ((Text)c).color = cp.ColorSettings[0].FinalColor },
            {
                typeof(Light), static (c, cp) =>
                {
                    ((Light)c).color = cp.ColorSettings[0].FinalColor;
                    cp.ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(Camera), static (c, cp) =>
                {
                    var cam = (Camera)c;
                    cam.clearFlags = CameraClearFlags.Color;
                    cam.backgroundColor = cp.ColorSettings[0].FinalColor;
                    cp.ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(ParticleSystem), static (c, cp) =>
                {
                    var ps = (ParticleSystem)c;
                    var main = ps.main;
                    main.startColor = cp.ColorSettings[0].FinalColor;
                }
            }
        };

        private static readonly int s_colorShaderId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _mpb;
        private Component _component;
        private Action _applyColorAction;

        private static void ApplyColorToSelectable(Component c, ColorProvider colorProvider)
        {
            var selectable = (Selectable)c;
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
            var renderer = (Renderer)c;
            var materials = renderer.sharedMaterials;

            if (materials.Length == 0)
            {
                Debug.LogWarning($"{renderer.GetType().Name} has no materials.", renderer);
                return;
            }

            colorProvider._mpb ??= new MaterialPropertyBlock();
            EnsureCorrectLength(colorProvider, materials.Length);

            for (int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material == null) continue;

                colorProvider.ColorSettings[i].Name = material.name;
                renderer.GetPropertyBlock(colorProvider._mpb, i);
                colorProvider._mpb.SetColor(s_colorShaderId, colorProvider.ColorSettings[i].FinalColor);
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

        private static bool UsesAlpha(Material material) =>
            material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") ||
            material.IsKeywordEnabled("_ALPHATEST_ON");

        private void OnValidate() => Init();

        [Button]
        public void Init()
        {
            if (!enabled) return;

            _ = GetSceneSettings();

            if (ColorSettings.Count == 0)
            {
                ColorSettings.Add(new ColorSettings());
            }

            if (!_init)
            {
                GetPaletteSaturationAndValue();
                _init = true;
                return;
            }

            ApplyColorAction();
        }

        [Button]
        public void GetPaletteSaturationAndValue()
        {
            if (!GetSceneSettings()) return;

            if (TryGetComponent<Renderer>(out var rend) && rend.sharedMaterials.Length > 0)
            {
                var materials = rend.sharedMaterials;
                EnsureCorrectLength(this, materials.Length);

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && i < ColorSettings.Count)
                    {
                        ColorSettings[i].Name = materials[i].name;
                    }
                }
            }

            const int hueTypeCount = 3; // Base, Tone, Accent

            for (int i = 0; i < ColorSettings.Count; i++)
            {
                if (!_init && i > 0)
                {
                    ColorSettings[i].HueType = (HueType)(((int)ColorSettings[0].HueType + i) % hueTypeCount);
                }

                var satVal = _pletSceneSettings.GetColorSV(ColorSettings[i].HueType);
                ColorSettings[i].Saturation = satVal.x;
                ColorSettings[i].Value = satVal.y;
                ColorSettings[i].Alpha = 1f;
                ColorSettings[i].FinalColor = _pletSceneSettings.ApplyColor(
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
            if (_pletSceneSettings != null) return true;

            _pletSceneSettings = PletHelpers.GetPletSceneSettings();
            return _pletSceneSettings != null;
        }

        private void CacheComponent()
        {
            _component = null;
            _applyColorAction = null;

            foreach (var kvp in s_colorAppliers)
            {
                if (!TryGetComponent(kvp.Key, out var component)) continue;

                _component = component;
                _applyColorAction = () => kvp.Value(component, this);
                break;
            }
        }

        [Button]
        public void ApplyColorAction()
        {
            if (!enabled || !GetSceneSettings()) return;

            if (_component == null || _applyColorAction == null)
            {
                CacheComponent();
            }

            if (_applyColorAction == null)
            {
                Debug.LogWarning("Component type not supported or not found.", this);
                return;
            }

            if (_pletSceneSettings?.Palette == null)
            {
                Debug.LogWarning("Cannot apply colors: PletSceneSettings or Palette is null.", this);
                return;
            }

            foreach (var setting in ColorSettings)
            {
                setting.FinalColor = _pletSceneSettings.ApplyColor(
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
