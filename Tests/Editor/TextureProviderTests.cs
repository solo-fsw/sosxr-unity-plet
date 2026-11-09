using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SOSXR.plet.Tests
{
    public class TextureProviderTests
    {
        private static readonly int s_baseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int s_mainTexId = Shader.PropertyToID("_MainTex");

        private PletTestUtility _util;
        private Func<string, Texture2D> _originalLoader;
        private Dictionary<string, Texture2D> _textures;

        [SetUp]
        public void SetUp()
        {
            _util = new PletTestUtility();
            _originalLoader = TextureProvider.TextureLoader;
            _textures = new Dictionary<string, Texture2D>();
            TextureProvider.TextureLoader = path => _textures.TryGetValue(path, out var t) ? t : null;
        }

        [TearDown]
        public void TearDown()
        {
            TextureProvider.TextureLoader = _originalLoader;
            _util.DestroyAll();
        }

        private string[] AddVariants(string baseName, bool legacyLocation = false)
        {
            var names = new string[TextureProvider.TextureSaturationSteps];

            for (int i = 0; i < names.Length; i++)
            {
                string fileName = PletHelpers.DesaturatedTextureName(baseName, PletHelpers.SaturationPercentageForStep(i, names.Length));
                string path = legacyLocation ? fileName : PletHelpers.TextureResourcePath(fileName);
                var texture = _util.Track(new Texture2D(2, 2) { name = fileName });
                _textures[path] = texture;
                names[i] = path;
            }

            return names;
        }

        private Material CreateMaterial()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) Assert.Ignore("No lit shader available.");

            return _util.Track(new Material(shader));
        }

        private static int MainTextureId(Material material) => material.HasProperty(s_baseMapId) ? s_baseMapId : s_mainTexId;

        private (TextureProvider provider, MeshRenderer renderer) CreateProvider(params Material[] materials)
        {
            var go = _util.Track(new GameObject());
            go.SetActive(false);
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = materials;
            var provider = go.AddComponent<TextureProvider>();

            return (provider, renderer);
        }

        [Test]
        public void GetNextTexture_UsesMaterialIndexAndPropertyBlock_NotTheSharedMaterial()
        {
            var untextured = CreateMaterial();
            var textured = CreateMaterial();
            var originalTexture = _util.Track(new Texture2D(2, 2));
            textured.mainTexture = originalTexture;

            var (provider, renderer) = CreateProvider(untextured, textured);
            var names = AddVariants("wood");

            provider.TextureSettings.Add(new TextureSettings
            {
                MaterialIndex = 1,
                Index = 3,
                PreviousIndex = 10,
                TextureNames = names
            });

            provider.GetNextTexture();

            var expected = _textures[names[3]];
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block, 1);

            Assert.That(provider.TextureSettings[0].CurrentTexture, Is.SameAs(expected));
            Assert.That(block.GetTexture(MainTextureId(textured)), Is.SameAs(expected));
            Assert.That(textured.mainTexture, Is.SameAs(originalTexture), "Shared material asset must not be modified.");
            Assert.That(provider.TextureSettings[0].PreviousIndex, Is.EqualTo(3));
        }

        [Test]
        public void GetNextTexture_IndexOutOfRange_DoesNotThrow()
        {
            var (provider, _) = CreateProvider(CreateMaterial());
            provider.TextureSettings.Add(new TextureSettings
            {
                Index = 5,
                PreviousIndex = 0,
                TextureNames = Array.Empty<string>()
            });

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("out of range"));
            Assert.DoesNotThrow(provider.GetNextTexture);
        }

        [Test]
        public void GetNextTexture_MissingTexture_KeepsPreviousIndexSoItRetries()
        {
            var (provider, _) = CreateProvider(CreateMaterial());
            var names = new string[TextureProvider.TextureSaturationSteps];
            for (int i = 0; i < names.Length; i++) names[i] = "missing_" + i;

            provider.TextureSettings.Add(new TextureSettings { Index = 2, PreviousIndex = 10, TextureNames = names });

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Failed to load texture"));
            provider.GetNextTexture();

            Assert.That(provider.TextureSettings[0].PreviousIndex, Is.EqualTo(10));
        }

        [Test]
        public void ApplyAllTextures_ReappliesCurrentTextureOnEnable()
        {
            var material = CreateMaterial();
            var (provider, renderer) = CreateProvider(material);
            var current = _util.Track(new Texture2D(2, 2));

            provider.TextureSettings.Add(new TextureSettings
            {
                MaterialIndex = 0,
                Index = 10,
                PreviousIndex = 10,
                CurrentTexture = current,
                TextureNames = AddVariants("stone")
            });

            provider.gameObject.SetActive(true); // OnEnable

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block, 0);
            Assert.That(block.GetTexture(MainTextureId(material)), Is.SameAs(current));
        }

        [Test]
        public void FindExistingDesaturatedTextures_FindsAllVariantsInTextureFolder()
        {
            var names = AddVariants("brick");

            Assert.That(TextureProvider.FindExistingDesaturatedTextures("brick", TextureProvider.TextureLoader), Is.EqualTo(names));
        }

        [Test]
        public void FindExistingDesaturatedTextures_AcceptsAVariantNameAsInput()
        {
            var names = AddVariants("brick");

            Assert.That(TextureProvider.FindExistingDesaturatedTextures("brick_saturated_40", TextureProvider.TextureLoader), Is.EqualTo(names));
        }

        [Test]
        public void FindExistingDesaturatedTextures_FallsBackToLegacyResourcesRoot()
        {
            var names = AddVariants("legacy", legacyLocation: true);

            Assert.That(TextureProvider.FindExistingDesaturatedTextures("legacy", TextureProvider.TextureLoader), Is.EqualTo(names));
        }

        [Test]
        public void FindExistingDesaturatedTextures_IncompleteSet_ReturnsNull()
        {
            var names = AddVariants("partial");
            _textures.Remove(names[4]);

            Assert.That(TextureProvider.FindExistingDesaturatedTextures("partial", TextureProvider.TextureLoader), Is.Null);
        }
    }
}
