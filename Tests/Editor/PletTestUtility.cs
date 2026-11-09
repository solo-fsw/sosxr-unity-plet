using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.plet.Tests
{
    /// <summary>Creates in-memory palettes, settings and GameObjects, and destroys them after a test.</summary>
    internal sealed class PletTestUtility
    {
        private readonly List<Object> _created = new();

        public static readonly Color BaseColor = Color.red;
        public static readonly Color ToneColor = Color.green;
        public static readonly Color AccentColor = Color.blue;

        public PletSceneSettings CreateSettings()
        {
            var palette = Track(ScriptableObject.CreateInstance<Palette>());
            palette.Base = BaseColor;
            palette.Tone = ToneColor;
            palette.Accent = AccentColor;

            var settings = Track(ScriptableObject.CreateInstance<PletSceneSettings>());
            settings.Palette = palette;

            return settings;
        }

        /// <summary>
        ///     Creates an inactive GameObject with <typeparamref name="T"/> and a <see cref="ColorProvider"/> that
        ///     uses <paramref name="settings"/>, then activates it so <c>OnEnable</c> runs with those settings.
        /// </summary>
        public ColorProvider CreateProvider<T>(PletSceneSettings settings, out T component) where T : Component
        {
            var go = Track(new GameObject(typeof(T).Name));
            go.SetActive(false);
            component = go.AddComponent<T>();
            var provider = go.AddComponent<ColorProvider>();
            provider.UseSceneSettings(settings);
            go.SetActive(true);

            return provider;
        }

        public T Track<T>(T obj) where T : Object
        {
            _created.Add(obj);

            return obj;
        }

        public void DestroyAll()
        {
            for (int i = _created.Count - 1; i >= 0; i--)
            {
                if (_created[i] != null)
                {
                    Object.DestroyImmediate(_created[i]);
                }
            }

            _created.Clear();
        }

        public static void AssertColorsEqual(Color expected, Color actual, float tolerance = 0.01f)
        {
            NUnit.Framework.Assert.That(Mathf.Abs(expected.r - actual.r), NUnit.Framework.Is.LessThan(tolerance), $"r: expected {expected}, got {actual}");
            NUnit.Framework.Assert.That(Mathf.Abs(expected.g - actual.g), NUnit.Framework.Is.LessThan(tolerance), $"g: expected {expected}, got {actual}");
            NUnit.Framework.Assert.That(Mathf.Abs(expected.b - actual.b), NUnit.Framework.Is.LessThan(tolerance), $"b: expected {expected}, got {actual}");
            NUnit.Framework.Assert.That(Mathf.Abs(expected.a - actual.a), NUnit.Framework.Is.LessThan(tolerance), $"a: expected {expected}, got {actual}");
        }
    }
}
