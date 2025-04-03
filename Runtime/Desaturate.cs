using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace SOSXR.plet
{
    /// <summary>
    ///     Is used to desaturate a texture by a given percentage.
    ///     It will save the desaturated texture as a new PNG file in the SOSXR Resources folder.
    /// </summary>
    public static class Desaturate
    {
        public static string Suffix => "_saturated_";
        private static readonly string _folderPath = "Assets/_SOSXR/Resources";


        #if UNITY_EDITOR
        public static string Texture(Texture2D source, int saturationPercentage)
        {
            if (source == null)
            {
                Debug.LogError("No texture provided.");

                return null;
            }

            var assetPath = AssetDatabase.GetAssetPath(source);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogErrorFormat("Texture {0} must be an asset in the project.", source.name);

                return null;
            }

            if (!IsReadable(assetPath))
            {
                Debug.LogErrorFormat("Texture {0} must be marked as readable.", assetPath);

                return null;
            }

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var newFileName = $"{filename}{Suffix}{saturationPercentage}";

            var savePath = $"{_folderPath}/{newFileName}.png";

            var saturation = (float) Math.Round(saturationPercentage / 100f, 1);
            var desaturatedTexture = CalculateDesaturation(source, saturation);

            SaveAsPNG(desaturatedTexture, savePath);

            Debug.Log($"Desaturated texture saved to: {savePath}");
            AssetDatabase.Refresh();

            return newFileName;
        }


        private static bool IsReadable(string assetPath, bool force = false)
        {
            var textureImporter = (TextureImporter) AssetImporter.GetAtPath(assetPath);

            if (force && textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            return textureImporter.isReadable;
        }


        private static Texture2D CalculateDesaturation(Texture2D source, float saturation)
        {
            if (saturation is < 0 or > 1)
            {
                Debug.LogWarning("New Saturation must be between 0 and 1, will clamp to this range.");

                saturation = Mathf.Clamp(saturation, 0, 1);
            }

            int width = source.width, height = source.height;
            var result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = source.GetPixels();

            var desaturateBy = 1 - saturation;

            for (var i = 0; i < pixels.Length; i++)
            {
                var gray = pixels[i].r * 0.3f + pixels[i].g * 0.59f + pixels[i].b * 0.11f;
                pixels[i] = Color.Lerp(pixels[i], new Color(gray, gray, gray, pixels[i].a), desaturateBy);
            }

            result.SetPixels(pixels);
            result.Apply();

            return result;
        }


        private static void SaveAsPNG(Texture2D texture, string filePath)
        {
            var pngData = texture.EncodeToPNG();

            if (pngData == null)
            {
                Debug.LogError("Failed to encode texture to PNG.");

                return;
            }

            File.WriteAllBytes(filePath, pngData);
            AssetDatabase.ImportAsset(filePath);
        }


        public static Texture2D LoadTexture(string name)
        {
            return Resources.Load<Texture2D>(name);
        }


        #endif
    }
}