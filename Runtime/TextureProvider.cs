using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOSXR.plet
{
    /// <summary>
    ///     MonoBehaviour that manages a set of pre-generated desaturated texture variants for a <see cref="Renderer"/>.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public sealed class TextureProvider : MonoBehaviour
    {
        public List<TextureSettings> TextureSettings = new(1);

        [HideInInspector][SerializeField] private bool _init;
        private Renderer _renderer;
        private static bool s_isDesaturating;

        public const int TextureSaturationSteps = 11;
        public static Func<Texture2D, int, string> DesaturateTexture { get; set; }

        private void OnValidate()
        {
            _renderer ??= GetComponent<Renderer>();
            TextureSettings ??= new List<TextureSettings>();

#if UNITY_EDITOR
            EditorApplication.delayCall += Initialize;
#endif

            if (!_init) return;
            GetNextTexture();
        }

        public void GetNextTexture()
        {
            var materials = _renderer.sharedMaterials;
            var texturesToLoad = new Dictionary<string, int>();

            // Collect all textures that need loading (issue #4)
            for (int index = 0; index < TextureSettings.Count; index++)
            {
                var settings = TextureSettings[index];
                if (settings.Index != settings.PreviousIndex)
                {
                    var textureName = settings.TextureNames[settings.Index];
                    if (!string.IsNullOrEmpty(textureName) && !texturesToLoad.ContainsKey(textureName))
                    {
                        texturesToLoad[textureName] = index;
                    }
                }
            }

            var loadedTextures = new Dictionary<string, Texture2D>();
            foreach (var texName in texturesToLoad.Keys)
            {
                var loaded = Resources.Load<Texture2D>(texName);
                if (loaded != null)
                    loadedTextures[texName] = loaded;
                else
                    Debug.LogWarning($"Failed to load texture '{texName}' from Resources.", this);
            }

            // Apply loaded textures
            for (int index = 0; index < TextureSettings.Count; index++)
            {
                var settings = TextureSettings[index];
                if (settings.Index == settings.PreviousIndex) continue;

                var textureName = settings.TextureNames[settings.Index];
                if (loadedTextures.TryGetValue(textureName, out var loadedTexture))
                {
                    settings.CurrentTexture = loadedTexture;
                    if (index < materials.Length && materials[index] != null)
                    {
                        materials[index].mainTexture = loadedTexture;
                    }
                    settings.PreviousIndex = settings.Index;
                }
                else if (string.IsNullOrEmpty(textureName))
                {
                    Debug.LogWarning($"Texture name is null or empty for material slot {index}.", this);
                }
            }
        }

        [ContextMenu(nameof(Initialize))]
        public void Initialize()
        {
            if (_init) return;

            var materials = _renderer.sharedMaterials;

            foreach (var sharedMat in materials)
            {
                if (sharedMat?.mainTexture == null) continue;

                var settings = new TextureSettings
                {
                    CurrentTexture = (Texture2D)sharedMat.mainTexture,
                    MaterialName = sharedMat.name,
                    TextureNames = GetDesaturatedTextures((Texture2D)sharedMat.mainTexture)
                };

                TextureSettings.Add(settings);
            }

            _init = true;
        }

        private string[] GetDesaturatedTextures(Texture2D currentTexture)
        {
            if (s_isDesaturating)
            {
                Debug.LogWarning("Already desaturating textures. Only one component at a time.", this);
                return System.Array.Empty<string>();
            }

            if (DesaturationHasBeenDone(currentTexture.name))
            {
                return FindTextures(currentTexture.name).Select(t => t.name).ToArray();
            }

            s_isDesaturating = true;

            try
            {
                string[] textureNames = new string[TextureSaturationSteps];

                for (int i = 0; i < TextureSaturationSteps; i++)
                {
                    int saturationPercentage = Mathf.RoundToInt((float)i / (TextureSaturationSteps - 1) * 100);
                    var result = DesaturateTexture?.Invoke(currentTexture, saturationPercentage);

                    if (string.IsNullOrEmpty(result))
                    {
                        throw new InvalidOperationException(
                            $"Failed to desaturate texture '{currentTexture.name}' at saturation {saturationPercentage}%.");
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

        private static bool DesaturationHasBeenDone(string textureName) => 
            FindTextures(textureName).Length != 0;

        private static Texture2D[] FindTextures(string textureName)
        {
            if (textureName.Contains(PletHelpers.Suffix))
            {
                int suffixIndex = textureName.IndexOf(PletHelpers.Suffix, StringComparison.Ordinal);
                if (suffixIndex >= 0)
                {
                    textureName = textureName.Substring(0, suffixIndex);
                }
            }

            var results = new List<Texture2D>();
            var textures = Resources.LoadAll<Texture2D>("Plet/Textures");
            var pattern = textureName + PletHelpers.Suffix;

            foreach (var t in textures)
            {
                if (t.name.StartsWith(pattern))
                    results.Add(t);
            }

            var sorted = results.ToArray();
            System.Array.Sort(sorted, (a, b) => ExtractNumber(a.name).CompareTo(ExtractNumber(b.name)));

            return sorted;
        }

        private static readonly Regex s_numberRegex = new(@"\d+", RegexOptions.Compiled);

        private static int ExtractNumber(string name)
        {
            var matches = s_numberRegex.Matches(name);
            return matches.Count == 0 ? int.MaxValue : int.Parse(matches[matches.Count - 1].Value);
        }
    }
}
