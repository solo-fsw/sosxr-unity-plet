using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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
        [HideInInspector] [SerializeField] private PaletteHolder _paletteHolder;

        private static readonly Dictionary<Type, Action<Component, Color>> ColorAppliers = new()
        {
            {
                typeof(SpriteRenderer), (c, col) => ((SpriteRenderer) c).color = col
            },
            {
                typeof(Renderer), (c, _) => ApplyColorToRenderer((Renderer) c, c.GetComponent<ColorProvider>())
            },
            {
                typeof(Selectable), (c, _) => ApplyColorToSelectable((Selectable) c, c.GetComponent<ColorProvider>())
            },
            {
                typeof(Image), (c, col) =>
                {
                    var img = (Image) c;
                    img.color = col;
                }
            },
            {
                typeof(TMP_Text), (c, col) =>
                {
                    var text = (TMP_Text) c;
                    text.color = col;
                }
            },
            {
                typeof(Light), (c, col) =>
                {
                    ((Light) c).color = col;
                    c.GetComponent<ColorProvider>().ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(Camera), (c, col) =>
                {
                    ((Camera) c).clearFlags = CameraClearFlags.Color;
                    ((Camera) c).backgroundColor = col;
                    c.GetComponent<ColorProvider>().ColorSettings[0].ShowAlpha = false;
                }
            },
            {
                typeof(ParticleSystem), (c, col) =>
                {
                    var main = ((ParticleSystem) c).main;
                    main.startColor = col;
                }
            }
        };


        private readonly int _colorShaderId = Shader.PropertyToID("_BaseColor");

        private Component _component;
        private Action<Color> _applyColorAction;
        private MaterialPropertyBlock _mpb;
        private HueType _cachedHueType;


        private static void ApplyColorToSelectable(Selectable selectable, ColorProvider colorProvider)
        {
            var colors = selectable.colors;

            while (colorProvider.ColorSettings.Count < 5)
            {
                colorProvider.ColorSettings.Add(new ColorSettings());
            }

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


        private static void ApplyColorToRenderer(Renderer renderer, ColorProvider colorProvider)
        {
            colorProvider._mpb ??= new MaterialPropertyBlock();

            if (renderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning($"{renderer.GetType().Name} with no materials not supported.");

                return;
            }

            EnsureCorrectLength(colorProvider, renderer.sharedMaterials.Length);

            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                colorProvider.ColorSettings[i].Name = renderer.sharedMaterials[i].name;
                renderer.GetPropertyBlock(colorProvider._mpb, i);
                colorProvider._mpb.SetColor(colorProvider._colorShaderId, colorProvider.ColorSettings[i].FinalColor);
                renderer.SetPropertyBlock(colorProvider._mpb, i);
                colorProvider.ColorSettings[i].ShowAlpha = UsesAlpha(renderer.sharedMaterials[i]);
            }
        }


        private static void EnsureCorrectLength(ColorProvider colorProvider, int lengthNeeded)
        {
            // Fix: Create individual ColorSettings instances instead of reusing the same one
            if (colorProvider.ColorSettings.Count < lengthNeeded)
            {
                // Create separate instances for each new ColorSettings
                for (var i = colorProvider.ColorSettings.Count; i < lengthNeeded; i++)
                {
                    colorProvider.ColorSettings.Add(new ColorSettings());
                }
            }
            else if (colorProvider.ColorSettings.Count > lengthNeeded)
            {
                colorProvider.ColorSettings.RemoveRange(lengthNeeded, colorProvider.ColorSettings.Count - lengthNeeded);
            }
        }


        private static bool UsesAlpha(Material material)
        {
            return material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") || material.IsKeywordEnabled("_ALPHATEST_ON");
        }


        private void OnValidate()
        {
            #if UNITY_EDITOR
            EditorApplication.delayCall += Init;
            #endif
        }


        private void Init()
        {
            if (enabled == false)
            {
                return;
            }

            GetPaletteHolder();

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


        [ContextMenu(nameof(GetPaletteSaturationAndValue))]
        public void GetPaletteSaturationAndValue()
        {
            GetPaletteHolder(); // Ensure we have a palette holder

            // Make sure we have the right number of settings for the attached renderer
            if (TryGetComponent<Renderer>(out var rend) && rend.sharedMaterials.Length > 0)
            {
                EnsureCorrectLength(this, rend.sharedMaterials.Length);

                // Initialize the names from the materials
                for (var i = 0; i < rend.sharedMaterials.Length; i++)
                {
                    if (i < ColorSettings.Count)
                    {
                        ColorSettings[i].Name = rend.sharedMaterials[i].name;
                    }
                }
            }

            // Initialize all color settings properly
            for (var i = 0; i < ColorSettings.Count; i++)
            {
                // If this is the first initialization, assign a default hue type 
                // that varies for each material to create visual distinction
                if (!init && i > 0)
                {
                    // Distribute hue types across materials (cycling through enum values)
                    var hueTypeCount = Enum.GetValues(typeof(HueType)).Length;
                    ColorSettings[i].HueType = (HueType) ((int) (ColorSettings[0].HueType + i) % hueTypeCount);
                }

                var satVal = _paletteHolder.GetColorSV(ColorSettings[i].HueType);
                ColorSettings[i].Saturation = satVal.x;
                ColorSettings[i].Value = satVal.y;
                ColorSettings[i].Alpha = 1f;

                // Calculate the final color immediately
                ColorSettings[i].FinalColor = _paletteHolder.ApplyColor(
                    ColorSettings[i].HueType,
                    ColorSettings[i].Saturation,
                    ColorSettings[i].Value,
                    ColorSettings[i].Alpha
                );
            }

            ApplyColorAction();
        }


        private void GetPaletteHolder()
        {
            if (_paletteHolder != null)
            {
                return;
            }

            var pletHelpers = new pletHelpers();

            _paletteHolder = pletHelpers.GetPaletteHolder(gameObject);
        }


        private void TryGetComponent()
        {
            _component = null;

            if (TryGetComponent(out Light lite))
            {
                _component = lite;
            }
            else if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                _component = spriteRenderer;
            }
            else if (TryGetComponent<Renderer>(out var rend))
            {
                _component = rend;
            }
            else if (TryGetComponent(out Camera cam))
            {
                _component = cam;
            }
            else if (TryGetComponent(out ParticleSystem ps))
            {
                _component = ps;
            }
            else if (TryGetComponent<Selectable>(out var selectable))
            {
                _component = selectable;
            }
            else if (TryGetComponent<Image>(out var img))
            {
                _component = img;
            }
            else if (TryGetComponent<TMP_Text>(out var text))
            {
                _component = text;
            }

            if (_component == null)
            {
                _applyColorAction = null;

                return;
            }

            foreach (var entry in ColorAppliers.Where(entry => _component.GetType() == entry.Key || _component.GetType().IsSubclassOf(entry.Key)))
            {
                _applyColorAction = color => entry.Value(_component, color);

                return;
            }
        }


        private void OnEnable()
        {
            if (_paletteHolder == null || !enabled)
            {
                return;
            }

            _paletteHolder.OnPaletteChanged += ApplyColorAction;
        }


        private void ApplyColorAction()
        {
            if (!enabled)
            {
                return;
            }

            TryGetComponent();

            if (_applyColorAction == null)
            {
                Debug.LogWarningFormat(this, "Component type not supported, or not found.");

                return;
            }

            var colorSettingsCopy = new List<ColorSettings>(ColorSettings); // Create a copy to iterate

            foreach (var setting in colorSettingsCopy)
            {
                setting.FinalColor = _paletteHolder.ApplyColor(setting.HueType, setting.Saturation, setting.Value, setting.Alpha);
                _applyColorAction.Invoke(setting.FinalColor);
            }
        }


        private void OnDisable()
        {
            if (_paletteHolder == null) // Should I check for enabled? That seems like a bad idea.
            {
                return;
            }

            _paletteHolder.OnPaletteChanged -= ApplyColorAction;
        }
    }
}