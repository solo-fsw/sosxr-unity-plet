using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOSXR.plet
{
    /// <summary>
    ///     MonoBehaviour that manages a set of pre-generated desaturated texture variants for a <see cref="Renderer"/>.
    ///     The chosen variant is applied per material slot through a <see cref="MaterialPropertyBlock"/>, so the
    ///     shared material asset on disk is never modified. Textures are (re)applied in <c>OnEnable</c>, so they
    ///     also show up in builds.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public sealed class TextureProvider : MonoBehaviour
    {
        public List<TextureSettings> TextureSettings = new(1);

        [HideInInspector][SerializeField] private bool _init;
        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;
        private static bool s_isDesaturating;

        private static readonly int s_baseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int s_mainTexId = Shader.PropertyToID("_MainTex");

        public const int TextureSaturationSteps = 11;

        /// <summary>
        ///     Creates one desaturated variant of a texture and returns its Resources path, or null on failure.
        ///     Set by the editor assembly (<c>Desaturate.Texture</c>); null in builds.
        /// </summary>
        public static Func<Texture2D, int, string> DesaturateTexture { get; set; }

        /// <summary>Loads a texture by Resources path. Defaults to <see cref="Resources.Load{T}(string)"/>; replaceable for tests.</summary>
        public static Func<string, Texture2D> TextureLoader { get; set; } = Resources.Load<Texture2D>;

        /// <summary>Whether this provider has built its <see cref="TextureSettings"/> list.</summary>
        public bool IsInitialized => _init;

        private Renderer Renderer => _renderer != null ? _renderer : _renderer = GetComponent<Renderer>();

        private void OnEnable() => ApplyAllTextures();

        private void OnValidate()
        {
            TextureSettings ??= new List<TextureSettings>();

#if UNITY_EDITOR
            if (!_init)
            {
                EditorApplication.delayCall += () =>
                {
                    if (this != null) Initialize();
                };

                return;
            }
#endif
            if (_init) GetNextTexture();
        }

        /// <summary>
        ///     Loads and applies the texture for every slot whose <see cref="SOSXR.plet.TextureSettings.Index"/>
        ///     changed since the last call.
        /// </summary>
        public void GetNextTexture() => ApplyTextures(onlyChanged: true);

        /// <summary>Applies the current texture of every slot, changed or not.</summary>
        public void ApplyAllTextures() => ApplyTextures(onlyChanged: false);

        private void ApplyTextures(bool onlyChanged)
        {
            if (Renderer == null || TextureSettings == null) return;

            var materials = Renderer.sharedMaterials;

            foreach (var settings in TextureSettings)
            {
                if (settings == null) continue;

                bool changed = settings.Index != settings.PreviousIndex;

                if (changed)
                {
                    if (!TryLoadSelectedTexture(settings, out var texture))
                    {
                        continue;
                    }

                    settings.CurrentTexture = texture;
                    settings.PreviousIndex = settings.Index;
                }
                else if (onlyChanged)
                {
                    continue;
                }

                ApplyToSlot(materials, settings.MaterialIndex, settings.CurrentTexture);
            }
        }

        private bool TryLoadSelectedTexture(TextureSettings settings, out Texture2D texture)
        {
            texture = null;

            if (settings.TextureNames == null || settings.Index < 0 || settings.Index >= settings.TextureNames.Length)
            {
                Debug.LogWarning($"Saturation step {settings.Index} is out of range for material slot {settings.MaterialIndex}.", this);
                return false;
            }

            string textureName = settings.TextureNames[settings.Index];

            if (string.IsNullOrEmpty(textureName))
            {
                Debug.LogWarning($"Texture name is null or empty for material slot {settings.MaterialIndex}.", this);
                return false;
            }

            texture = TextureLoader?.Invoke(textureName);

            if (texture == null)
            {
                Debug.LogWarning($"Failed to load texture '{textureName}' from Resources.", this);
                return false;
            }

            return true;
        }

        private void ApplyToSlot(Material[] materials, int materialIndex, Texture texture)
        {
            if (texture == null || materialIndex < 0 || materialIndex >= materials.Length || materials[materialIndex] == null)
            {
                return;
            }

            int propertyId = GetMainTexturePropertyId(materials[materialIndex]);
            if (propertyId == 0) return;

            _mpb ??= new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(_mpb, materialIndex);
            _mpb.SetTexture(propertyId, texture);
            Renderer.SetPropertyBlock(_mpb, materialIndex);
        }

        private static int GetMainTexturePropertyId(Material material)
        {
            if (material.HasProperty(s_baseMapId)) return s_baseMapId;
            if (material.HasProperty(s_mainTexId)) return s_mainTexId;
            return 0;
        }

        /// <summary>
        ///     Builds one <see cref="SOSXR.plet.TextureSettings"/> entry per textured material slot, generating the
        ///     desaturated variants if they do not exist yet. Does nothing once initialized.
        /// </summary>
        [ContextMenu(nameof(Initialize))]
        public void Initialize()
        {
            if (_init || Renderer == null) return;

            var materials = Renderer.sharedMaterials;
            var newSettings = new List<TextureSettings>();

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null || materials[i].mainTexture is not Texture2D texture) continue;

                var names = GetDesaturatedTextures(texture);

                if (names.Length != TextureSaturationSteps)
                {
                    // Desaturation failed or is busy. Leave uninitialized so it is retried later.
                    return;
                }

                newSettings.Add(new TextureSettings
                {
                    CurrentTexture = texture,
                    MaterialName = materials[i].name,
                    MaterialIndex = i,
                    Index = TextureSaturationSteps - 1,
                    PreviousIndex = TextureSaturationSteps - 1,
                    TextureNames = names
                });
            }

#if UNITY_EDITOR
            Undo.RecordObject(this, "Initialize TextureProvider");
#endif
            TextureSettings.Clear();
            TextureSettings.AddRange(newSettings);
            _init = true;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        ///     Rebuilds <see cref="TextureSettings"/> from the renderer's current materials. Use after changing
        ///     materials, or to upgrade data saved by versions before 0.5.0 (which had no material slot index).
        /// </summary>
        [ContextMenu(nameof(Reinitialize))]
        public void Reinitialize()
        {
            _init = false;
            Initialize();
            ApplyAllTextures();
        }

        private string[] GetDesaturatedTextures(Texture2D currentTexture)
        {
            if (s_isDesaturating)
            {
                Debug.LogWarning("Already desaturating textures. Only one component at a time.", this);
                return Array.Empty<string>();
            }

            var existing = FindExistingDesaturatedTextures(currentTexture.name, TextureLoader);
            if (existing != null) return existing;

            if (DesaturateTexture == null)
            {
                Debug.LogWarning("No desaturation function available (editor only).", this);
                return Array.Empty<string>();
            }

            s_isDesaturating = true;

            try
            {
                string[] textureNames = new string[TextureSaturationSteps];

                for (int i = 0; i < TextureSaturationSteps; i++)
                {
                    int saturationPercentage = PletHelpers.SaturationPercentageForStep(i, TextureSaturationSteps);
                    string result = DesaturateTexture(currentTexture, saturationPercentage);

                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.LogError($"Failed to desaturate texture '{currentTexture.name}' at saturation {saturationPercentage}%.", this);
                        return Array.Empty<string>();
                    }

                    textureNames[i] = result;
                }

                return textureNames;
            }
            finally
            {
                s_isDesaturating = false;
            }
        }

        /// <summary>
        ///     Returns the Resources paths of all <see cref="TextureSaturationSteps"/> desaturated variants of
        ///     <paramref name="textureName"/> if every one of them already exists, otherwise null.
        ///     Looks in <see cref="PletHelpers.TextureResourcesFolder"/> first, then in the Resources root
        ///     (where versions before 0.5.0 saved them).
        /// </summary>
        public static string[] FindExistingDesaturatedTextures(string textureName, Func<string, Texture2D> loader)
        {
            if (string.IsNullOrEmpty(textureName) || loader == null) return null;

            int suffixIndex = textureName.IndexOf(PletHelpers.Suffix, StringComparison.Ordinal);
            string baseName = suffixIndex >= 0 ? textureName.Substring(0, suffixIndex) : textureName;

            string[] names = new string[TextureSaturationSteps];

            for (int i = 0; i < TextureSaturationSteps; i++)
            {
                string fileName = PletHelpers.DesaturatedTextureName(baseName, PletHelpers.SaturationPercentageForStep(i, TextureSaturationSteps));
                string currentPath = PletHelpers.TextureResourcePath(fileName);

                if (loader(currentPath) != null)
                {
                    names[i] = currentPath;
                }
                else if (loader(fileName) != null)
                {
                    names[i] = fileName;
                }
                else
                {
                    return null;
                }
            }

            return names;
        }
    }
}
