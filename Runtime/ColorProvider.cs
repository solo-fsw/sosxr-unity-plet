using System;
using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.Plet
{
    [ExecuteInEditMode]
    public class ColorProvider : MonoBehaviour
    {
        [SerializeField] private List<ColorSettings> m_colorSettings = new(1);

        [HideInInspector] [SerializeField] private bool init;
        [HideInInspector] [SerializeField] private PaletteHolder _paletteHolder;


        private static readonly Dictionary<Type, Action<Component, Color>> ColorAppliers = new()
        {
            {typeof(Light), (c, col) => ((Light) c).color = col},
            {typeof(SkinnedMeshRenderer), (c, col) => ApplyColorToRenderer((SkinnedMeshRenderer) c, c.GetComponent<ColorProvider>(), col)},
            {typeof(MeshRenderer), (c, col) => ApplyColorToRenderer((MeshRenderer) c, c.GetComponent<ColorProvider>(), col)},
            {typeof(Renderer), (c, col) => ((Renderer) c).sharedMaterial.color = col},
            {
                typeof(Camera), (c, col) =>
                {
                    ((Camera) c).clearFlags = CameraClearFlags.Color;
                    ((Camera) c).backgroundColor = col;
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
        private ColorType _cachedColorType;


        private static void ApplyColorToRenderer(Renderer renderer, ColorProvider colorProvider, Color col)
        {
            colorProvider._mpb ??= new MaterialPropertyBlock();

            if (renderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning($"{renderer.GetType().Name} with no materials not supported.");

                return;
            }

            if (colorProvider.m_colorSettings.Count < renderer.sharedMaterials.Length)
            {
                // Ensure correct list size before iteration
                while (colorProvider.m_colorSettings.Count < renderer.sharedMaterials.Length)
                {
                    colorProvider.m_colorSettings.Add(new ColorSettings());
                }
            }
            else if (colorProvider.m_colorSettings.Count > renderer.sharedMaterials.Length)
            {
                colorProvider.m_colorSettings.RemoveRange(renderer.sharedMaterials.Length, colorProvider.m_colorSettings.Count - renderer.sharedMaterials.Length);
            }

            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                colorProvider.m_colorSettings[i].Name = renderer.sharedMaterials[i].name;
                renderer.GetPropertyBlock(colorProvider._mpb, i);
                colorProvider._mpb.SetColor(colorProvider._colorShaderId, colorProvider.m_colorSettings[i].ValuedColor);
                renderer.SetPropertyBlock(colorProvider._mpb, i);
            }
        }


        private void OnValidate()
        {
            Init();
        }


        private void Reset()
        {
            init = false;

            Init();
        }


        private void Init()
        {
            if (enabled == false)
            {
                return;
            }

            GetPaletteHolder();

            if (!init)
            {
                if (m_colorSettings.Count == 0)
                {
                    m_colorSettings.Add(new ColorSettings());
                }

                GetBaseValues();

                init = true;

                return;
            }

            TryGetComponent();
            ApplyColorAction();
        }


        [ContextMenu(nameof(GetBaseValues))]
        private void GetBaseValues()
        {
            foreach (var setting in m_colorSettings)
            {
                setting.Value = 1f;
                setting.Saturation = 1f;
                setting.Alpha = 1f;
            }

            TryGetComponent();
            ApplyColorAction();
        }


        private void GetPaletteHolder()
        {
            var paletteHolders = Resources.LoadAll<PaletteHolder>("");

            if (paletteHolders.Length == 1)
            {
                _paletteHolder = paletteHolders[0];

                return;
            }

            if (paletteHolders.Length == 0)
            {
                Debug.LogWarning("No PaletteHolder found in any of the Resources folders.");

                return;
            }

            if (paletteHolders.Length > 1)
            {
                foreach (var paletteHolder in paletteHolders)
                {
                    if (paletteHolder.name == gameObject.scene.name)
                    {
                        _paletteHolder = paletteHolder;

                        break;
                    }
                }

                if (_paletteHolder == null)
                {
                    Debug.LogWarningFormat("Multiple PaletteHolders found in Resources folders, but none with the same name as the scene: {0}", gameObject.scene.name);
                }
            }
        }


        private void TryGetComponent()
        {
            _component = null;

            if (TryGetComponent<Light>(out var lite))
            {
                _component = lite;
            }
            else if (TryGetComponent<SkinnedMeshRenderer>(out var smr))
            {
                _component = smr;
            }
            else if (TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                _component = meshRenderer;
            }
            else if (TryGetComponent<Renderer>(out var rend))
            {
                _component = rend;
            }
            else if (TryGetComponent<Camera>(out var cam))
            {
                _component = cam;
            }
            else if (TryGetComponent<ParticleSystem>(out var ps))
            {
                _component = ps;
            }

            if (_component != null && ColorAppliers.TryGetValue(_component.GetType(), out var applyAction))
            {
                _applyColorAction = color => applyAction(_component, color);
            }
            else
            {
                _applyColorAction = null;
            }
        }


        private void OnEnable()
        {
            if (_paletteHolder != null)
            {
                _paletteHolder.OnPaletteChanged += ApplyColorAction;
            }
        }


        private void ApplyColorAction()
        {
            if (!enabled)
            {
                return;
            }

            if (_paletteHolder == null)
            {
                Debug.LogWarningFormat(this, "Cannot function without a PaletteHolder.");

                return;
            }

            if (_applyColorAction != null)
            {
                var colorSettingsCopy = new List<ColorSettings>(m_colorSettings); // Create a copy to iterate

                foreach (var setting in colorSettingsCopy)
                {
                    setting.ValuedColor = _paletteHolder.ApplyColor(setting.ColorType, setting.Value, setting.Saturation, setting.Alpha);
                    _applyColorAction.Invoke(setting.ValuedColor);
                }
            }
            else
            {
                Debug.LogWarningFormat(this, "Component type not supported, or not found.");
            }
        }


        private void OnDisable()
        {
            if (_paletteHolder != null)
            {
                _paletteHolder.OnPaletteChanged -= ApplyColorAction;
            }
        }
    }
}