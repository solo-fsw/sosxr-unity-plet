using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace SOSXR.plet
{
    [RequireComponent(typeof(Renderer))]
    public class TextureProvider : MonoBehaviour
    {
        public List<TextureSettings> TextureSettings = new(1);

        [HideInInspector] [SerializeField] private bool m_init = false;
        private Renderer _rend;
        private static bool _isDesaturating;

        public const int TextureSaturationSteps = 11;


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
                #if UNITY_EDITOR
                textureNames[i] = Desaturate.Texture(currentTexture, i * (TextureSaturationSteps - 1));
                #endif
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
            if (textureName.Contains(Desaturate.Suffix))
            {
                var last = Desaturate.Suffix.Substring(Desaturate.Suffix.Length - 1);
                var suffixIndex = textureName.LastIndexOf(last, StringComparison.Ordinal);
                textureName = textureName.Remove(suffixIndex);
            }

            var allContaining = Resources.LoadAll<Texture2D>("")
                                         .Where(t => t.name.Contains(textureName) && t.name.Contains(Desaturate.Suffix))
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