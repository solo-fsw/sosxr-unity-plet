using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.plet
{
    /// <summary>
    ///     Is used to desaturate a texture by a given percentage.
    ///     It will save the desaturated texture as a new PNG file in <see cref="PletHelpers.TextureFolderPath"/>,
    ///     which is the same folder <see cref="TextureProvider"/> loads from.
    /// </summary>
    public static class Desaturate
    {
        #if UNITY_EDITOR
        /// <summary>
        ///     Desaturates <paramref name="source"/> to <paramref name="saturationPercentage"/> percent of its original
        ///     saturation (0 = greyscale, 100 = fully saturated) and saves the result as a PNG in
        ///     <see cref="PletHelpers.TextureFolderPath"/>. Returns the Resources path of the new texture
        ///     (for <c>Resources.Load</c>), or <c>null</c> on failure.
        /// </summary>
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

            var folder = PletHelpers.TextureFolderPath;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var newFileName = PletHelpers.DesaturatedTextureName(filename, saturationPercentage);

            var savePath = $"{folder}/{newFileName}.png";

            var saturation = (float) Math.Round(saturationPercentage / 100f, 1);
            var desaturatedTexture = CalculateDesaturation(source, saturation);

            var saved = SaveAsPNG(desaturatedTexture, savePath);
            Object.DestroyImmediate(desaturatedTexture);

            if (!saved)
            {
                return null;
            }

            Debug.Log($"Desaturated texture saved to: {savePath}");

            return PletHelpers.TextureResourcePath(newFileName);
        }


        private static bool IsReadable(string assetPath, bool force = false)
        {
            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter == null)
            {
                Debug.LogError($"Failed to get TextureImporter for asset at path: {assetPath}");

                return false;
            }

            if (force && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            return textureImporter.isReadable;
        }


        /// <summary>
        ///     Returns a new texture where each pixel is blended towards its luminance grey.
        ///     <paramref name="saturation"/> 1 keeps the source colors, 0 gives greyscale. Alpha is preserved.
        ///     <paramref name="source"/> must be readable.
        /// </summary>
        public static Texture2D CalculateDesaturation(Texture2D source, float saturation)
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


        private static bool SaveAsPNG(Texture2D texture, string filePath)
        {
            var pngData = texture.EncodeToPNG();

            if (pngData == null)
            {
                Debug.LogError("Failed to encode texture to PNG.");

                return false;
            }

            File.WriteAllBytes(filePath, pngData);
            AssetDatabase.ImportAsset(filePath);

            return true;
        }


        /// <summary>Loads a <see cref="Texture2D"/> by resource name from the Resources folder.</summary>
        public static Texture2D LoadTexture(string name)
        {
            return Resources.Load<Texture2D>(name);
        }


        #endif
    }
}