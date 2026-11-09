using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOSXR.plet
{
    /// <summary>
    ///     MonoBehaviour that applies palette-driven colors to a supported component on the same GameObject.
    ///     Supports <see cref="ParticleSystem"/>, <see cref="TMP_Text"/>, <see cref="Text"/>,
    ///     <see cref="SpriteRenderer"/>, <see cref="Selectable"/>, <see cref="Image"/>, <see cref="Light"/>,
    ///     <see cref="Camera"/>, and any other <see cref="Renderer"/> (via MaterialPropertyBlock).
    ///     Colors are (re)applied in <c>OnEnable</c>, so they also show up in builds and after runtime
    ///     <c>AddComponent</c>, not only when the inspector validates.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class ColorProvider : MonoBehaviour
    {
        public List<ColorSettings> ColorSettings = new(1);

        /// <summary>
        ///     When the target is a <see cref="Camera"/>, also switch its clear flags to Solid Color so the
        ///     background color is visible. Disable to only set <see cref="Camera.backgroundColor"/>.
        /// </summary>
        [Tooltip("Camera only: switch the camera's Clear Flags to Solid Color so the background color is visible.")]
        public bool SetCameraClearFlagsToSolidColor = true;

        [SerializeField] private bool _init;
        [HideInInspector][SerializeField] private PletSceneSettings _pletSceneSettings;

        /// <summary>
        ///     Supported component types, in priority order. More specific types come first:
        ///     a TextMeshPro (3D) or ParticleSystem also has a Renderer, and a Button also has an Image.
        ///     The generic <see cref="Renderer"/> is checked last.
        /// </summary>
        private static readonly (Type Type, Action<Component, ColorProvider> Apply)[] s_colorAppliers =
        {
            (typeof(ParticleSystem), ApplyColorToParticleSystem),
            (typeof(TMP_Text), static (c, cp) => ((TMP_Text)c).color = cp.ColorSettings[0].FinalColor),
            (typeof(Text), static (c, cp) => ((Text)c).color = cp.ColorSettings[0].FinalColor),
            (typeof(SpriteRenderer), static (c, cp) => ((SpriteRenderer)c).color = cp.ColorSettings[0].FinalColor),
            (typeof(Selectable), ApplyColorToSelectable),
            (typeof(Image), static (c, cp) => ((Image)c).color = cp.ColorSettings[0].FinalColor),
            (typeof(Light), ApplyColorToLight),
            (typeof(Camera), ApplyColorToCamera),
            (typeof(Renderer), ApplyColorToRenderer)
        };

        private static readonly int s_colorShaderId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _mpb;
        private Component _component;
        private Action<Component, ColorProvider> _applier;
#if UNITY_EDITOR
        private bool _validatePending;
#endif

        /// <summary>Number of color slots a <see cref="Selectable"/> uses (normal, highlighted, pressed, selected, disabled).</summary>
        public const int SelectableSlotCount = 5;

        /// <summary>The component this provider colors, or <c>null</c> if none is supported.</summary>
        public Component Target
        {
            get
            {
                if (_component == null) CacheComponent();
                return _component;
            }
        }

        /// <summary>
        ///     Returns the supported component on <paramref name="gameObject"/> that a <see cref="ColorProvider"/>
        ///     would color, using the same priority order as the provider. Returns <c>null</c> if none is found.
        /// </summary>
        public static Component ResolveTarget(GameObject gameObject)
        {
            if (gameObject == null) return null;

            foreach (var (type, _) in s_colorAppliers)
            {
                if (gameObject.TryGetComponent(type, out var component)) return component;
            }

            return null;
        }

        /// <summary>
        ///     Use <paramref name="settings"/> instead of looking up the active <see cref="PletSceneSettings"/>
        ///     from Resources. Useful for tests and for setting up providers from code.
        /// </summary>
        public void UseSceneSettings(PletSceneSettings settings) => _pletSceneSettings = settings;

        private void OnEnable()
        {
            _component = null;
            _applier = null;
            Init();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            // Changing other components (and recording Undo) is not allowed inside OnValidate. Defer it.
            if (_validatePending) return;
            _validatePending = true;

            EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                _validatePending = false;
                Init();
            };
#endif
        }

        [Button]
        public void Init()
        {
            if (!enabled) return;

            if (ColorSettings.Count == 0)
            {
                ColorSettings.Add(new ColorSettings());
            }

            if (!_init && GetSceneSettings() && _pletSceneSettings.Palette != null)
            {
                _init = true;
                GetPaletteSaturationAndValue();
                return;
            }

            ApplyColorAction();
        }

        [Button]
        public void GetPaletteSaturationAndValue()
        {
            if (!GetSceneSettings() || _pletSceneSettings.Palette == null) return;

            if (TryGetComponent<Renderer>(out var rend) && Target is Renderer and not SpriteRenderer && rend.sharedMaterials.Length > 0)
            {
                var materials = rend.sharedMaterials;
                EnsureCorrectLength(this, materials.Length);

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null)
                    {
                        ColorSettings[i].Name = materials[i].name;
                    }
                }
            }

            for (int i = 0; i < ColorSettings.Count; i++)
            {
                var setting = ColorSettings[i];
                var satVal = _pletSceneSettings.GetColorSV(setting.HueType);
                setting.Saturation = satVal.x;
                setting.Value = satVal.y;
                setting.Alpha = 1f;
            }

            ApplyColorAction();
        }

        [Button]
        public void ApplyColorAction()
        {
            if (!enabled) return;

            if (_component == null || _applier == null)
            {
                CacheComponent();
            }

            if (_applier == null)
            {
                Debug.LogWarning("ColorProvider: no supported component found on this GameObject.", this);
                return;
            }

            if (ColorSettings.Count == 0)
            {
                ColorSettings.Add(new ColorSettings());
            }

            // Without settings or palette we still apply the serialized FinalColor, so a build without
            // a Resources PletSceneSettings keeps the colors that were authored in the editor.
            if (GetSceneSettings() && _pletSceneSettings.Palette != null)
            {
                foreach (var setting in ColorSettings)
                {
                    setting.FinalColor = _pletSceneSettings.ApplyColor(setting.HueType, setting.Saturation, setting.Value, setting.Alpha);
                }
            }

            ApplyWithUndo(_component, this, _applier);
        }

        private bool GetSceneSettings()
        {
            if (_pletSceneSettings != null) return true;

            try
            {
                _pletSceneSettings = PletHelpers.GetPletSceneSettings();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning(e.Message, this);
                return false;
            }

            return _pletSceneSettings != null;
        }

        private void CacheComponent()
        {
            _component = null;
            _applier = null;

            foreach (var (type, apply) in s_colorAppliers)
            {
                if (!TryGetComponent(type, out var component)) continue;

                _component = component;
                _applier = apply;
                break;
            }
        }

        private static void ApplyColorToParticleSystem(Component c, ColorProvider cp)
        {
            var main = ((ParticleSystem)c).main;
            main.startColor = cp.ColorSettings[0].FinalColor;
        }

        private static void ApplyColorToLight(Component c, ColorProvider cp)
        {
            ((Light)c).color = cp.ColorSettings[0].FinalColor;
            cp.ColorSettings[0].ShowAlpha = false;
        }

        private static void ApplyColorToCamera(Component c, ColorProvider cp)
        {
            var cam = (Camera)c;

            if (cp.SetCameraClearFlagsToSolidColor)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
            }

            cam.backgroundColor = cp.ColorSettings[0].FinalColor;
            cp.ColorSettings[0].ShowAlpha = false;
        }

        private static void ApplyColorToSelectable(Component c, ColorProvider colorProvider)
        {
            var selectable = (Selectable)c;
            var colors = selectable.colors;

            EnsureCorrectLength(colorProvider, SelectableSlotCount);

            var settings = colorProvider.ColorSettings;

            settings[0].Name = nameof(colors.normalColor);
            colors.normalColor = settings[0].FinalColor;

            settings[1].Name = nameof(colors.highlightedColor);
            colors.highlightedColor = settings[1].FinalColor;

            settings[2].Name = nameof(colors.pressedColor);
            colors.pressedColor = settings[2].FinalColor;

            settings[3].Name = nameof(colors.selectedColor);
            colors.selectedColor = settings[3].FinalColor;

            settings[4].Name = nameof(colors.disabledColor);
            colors.disabledColor = settings[4].FinalColor;

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

        /// <summary>
        ///     Grows or shrinks the slot list to <paramref name="lengthNeeded"/>. New slots cycle through the
        ///     hue types (starting after slot 0) and take their saturation/value from the palette, so they
        ///     never start as transparent black.
        /// </summary>
        private static void EnsureCorrectLength(ColorProvider colorProvider, int lengthNeeded)
        {
            var settings = colorProvider.ColorSettings;
            const int hueTypeCount = 3; // Base, Tone, Accent

            while (settings.Count < lengthNeeded)
            {
                var newSetting = new ColorSettings();

                if (settings.Count > 0)
                {
                    newSetting.HueType = (HueType)(((int)settings[0].HueType + settings.Count) % hueTypeCount);
                }

                if (colorProvider._pletSceneSettings != null && colorProvider._pletSceneSettings.Palette != null)
                {
                    var sv = colorProvider._pletSceneSettings.GetColorSV(newSetting.HueType);
                    newSetting.Saturation = sv.x;
                    newSetting.Value = sv.y;
                    newSetting.FinalColor = colorProvider._pletSceneSettings.ApplyColor(newSetting.HueType, sv.x, sv.y);
                }

                settings.Add(newSetting);
            }

            if (settings.Count > lengthNeeded)
            {
                settings.RemoveRange(lengthNeeded, settings.Count - lengthNeeded);
            }
        }

        private static bool UsesAlpha(Material material) =>
            material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") ||
            material.IsKeywordEnabled("_ALPHATEST_ON") ||
            material.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT");

        /// <summary>
        ///     Applies the color. In edit mode the change is recorded for Undo, and the object is marked dirty
        ///     only when its serialized data actually changed, so opening a scene does not dirty it.
        /// </summary>
        private static void ApplyWithUndo(Component target, ColorProvider provider, Action<Component, ColorProvider> apply)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string before = EditorJsonUtility.ToJson(target);
                Undo.RecordObject(target, "Apply plet color");
                apply(target, provider);

                if (EditorJsonUtility.ToJson(target) != before)
                {
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }

                return;
            }
#endif
            apply(target, provider);
        }
    }
}
