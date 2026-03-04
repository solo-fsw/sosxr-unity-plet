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
    ///     On initialisation it produces <see cref="TextureSaturationSteps"/> variants of each material's main texture
    ///     at evenly spaced saturation levels using <see cref="Desaturate"/>, then lets you scrub between them
    ///     at runtime via <see cref="TextureSettings.Index"/>.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class TextureProvider : MonoBehaviour
    {
        /// <summary>Per-material-slot configuration including the saturation index and resolved current texture reference.</summary>
        public List<TextureSettings> TextureSettings = new(1);

        [HideInInspector] [SerializeField] private bool m_init;
        private Renderer _rend;
        private static bool _isDesaturating;

        /// <summary>Number of discrete saturation steps generated per source texture (0 = fully desaturated through max = fully saturated).</summary>
        public const int TextureSaturationSteps = 11;

        /// <summary>
        ///     Editor-injected delegate that desaturates a source texture to the given saturation percentage
        ///     and returns the new asset name (without extension), or <c>null</c> on failure.
        ///     Registered by the Editor assembly at load time; <c>null</c> in builds.
        /// </summary>
        public static Func<Texture2D, int, string> DesaturateTexture;


        private void OnValidate()
        {
            _rend ??= GetComponent<Renderer>();

            TextureSettings ??= new List<TextureSettings>();

            #if UNITY_EDITOR
            EditorApplication.delayCall += Initialize;
            #endif

            if (!m_init)
            {
                return;
            }

            GetNextTexture();
        }


        /// <summary>
        ///     Checks each slot for an index change and loads the corresponding pre-generated texture variant
        ///     into the material's <c>mainTexture</c>. Call after modifying any <see cref="TextureSettings.Index"/>.
        /// </summary>
        public void GetNextTexture()
        {
            for (var index = 0; index < TextureSettings.Count; index++)
            {
                if (TextureSettings[index].Index == TextureSettings[index].PreviousIndex)
                {
                    continue;
                }

                TextureSettings[index].CurrentTexture = Resources.Load<Texture2D>(TextureSettings[index].TextureNames[TextureSettings[index].Index]);
                _rend.sharedMaterials[index].mainTexture = TextureSettings[index].CurrentTexture;
                TextureSettings[index].PreviousIndex = TextureSettings[index].Index;
            }
        }


        /// <summary>
        ///     Scans the Renderer's shared materials, registers each slot that has a main texture, and
        ///     generates all desaturated variants via <see cref="Desaturate"/>. No-op if already initialised.
        /// </summary>
        [ContextMenu(nameof(Initialize))]
        public void Initialize()
        {
            if (m_init)
            {
                return;
            }

            foreach (var sharedMat in _rend.sharedMaterials)
            {
                if (sharedMat.mainTexture == null)
                {
                    continue;
                }

                TextureSettings.Add(new TextureSettings
                {
                    CurrentTexture = (Texture2D) sharedMat.mainTexture
                });

                TextureSettings[^1].MaterialName = sharedMat.name;
                TextureSettings[^1].TextureNames = GetDesaturatedTextures(TextureSettings[^1].CurrentTexture);
            }

            m_init = true;
        }


        private string[] GetDesaturatedTextures(Texture2D currentTexture)
        {
            if (_isDesaturating)
            {
                Debug.LogWarningFormat("Already desaturating textures elsewhere, please wait for the process to finish. One component at a time please.");

                return null;
            }

            if (DesaturationHasBeenDone(currentTexture.name))
            {
                return FindTextures(currentTexture.name).Select(t => t.name).ToArray();
            }

            _isDesaturating = true;

            var textureNames = new string[TextureSaturationSteps];

            for (var i = 0; i < TextureSaturationSteps; i++)
            {
                textureNames[i] = DesaturateTexture?.Invoke(currentTexture, i * (TextureSaturationSteps - 1));
            }

            _isDesaturating = false;

            return textureNames;
        }


        private static bool DesaturationHasBeenDone(string textureName)
        {
            return FindTextures(textureName).Length != 0;
        }


        private static Texture2D[] FindTextures(string textureName)
        {
            if (textureName.Contains(PletHelpers.Suffix))
            {
                var last = PletHelpers.Suffix.Substring(PletHelpers.Suffix.Length - 1);
                var suffixIndex = textureName.LastIndexOf(last, StringComparison.Ordinal);
                textureName = textureName.Remove(suffixIndex);
            }

            var allContaining = Resources.LoadAll<Texture2D>("")
                                         .Where(t => t.name.Contains(textureName) && t.name.Contains(PletHelpers.Suffix))
                                         .OrderBy(t => ExtractNumber(t.name))
                                         .ToArray();

            return allContaining;
        }


        private static int ExtractNumber(string name)
        {
            var matches = Regex.Matches(name, @"\d+");
            var lastMatch = matches[^1];

            return matches.Count > 0 ? int.Parse(lastMatch.Value) : int.MaxValue;
        }
    }
}