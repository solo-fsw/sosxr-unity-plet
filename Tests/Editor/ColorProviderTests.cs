using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SOSXR.plet.Tests
{
    public class ColorProviderTests
    {
        private static readonly int s_baseColorId = Shader.PropertyToID("_BaseColor");

        private PletTestUtility _util;
        private PletSceneSettings _settings;

        [SetUp]
        public void SetUp()
        {
            _util = new PletTestUtility();
            _settings = _util.CreateSettings();
        }

        [TearDown]
        public void TearDown() => _util.DestroyAll();

        [Test]
        public void ResolveTarget_ParticleSystemWinsOverItsRenderer()
        {
            var go = _util.Track(new GameObject());
            var ps = go.AddComponent<ParticleSystem>();

            Assert.That(go.GetComponent<Renderer>(), Is.Not.Null, "ParticleSystem should add a ParticleSystemRenderer.");
            Assert.That(ColorProvider.ResolveTarget(go), Is.SameAs(ps));
        }

        [Test]
        public void ResolveTarget_TextMeshProWinsOverItsMeshRenderer()
        {
            LogAssert.ignoreFailingMessages = true; // TMP may complain about missing font assets in a bare project.

            var go = _util.Track(new GameObject());
            var tmp = go.AddComponent<TextMeshPro>();

            Assert.That(go.GetComponent<Renderer>(), Is.Not.Null, "TextMeshPro should add a MeshRenderer.");
            Assert.That(ColorProvider.ResolveTarget(go), Is.SameAs(tmp));
        }

        [Test]
        public void ResolveTarget_ButtonWinsOverItsImage()
        {
            var go = _util.Track(new GameObject());
            go.AddComponent<Image>();
            var button = go.AddComponent<Button>();

            Assert.That(ColorProvider.ResolveTarget(go), Is.SameAs(button));
        }

        [Test]
        public void ResolveTarget_PlainMeshRenderer_IsUsed()
        {
            var go = _util.Track(new GameObject());
            var renderer = go.AddComponent<MeshRenderer>();

            Assert.That(ColorProvider.ResolveTarget(go), Is.SameAs(renderer));
        }

        [Test]
        public void ResolveTarget_NoSupportedComponent_ReturnsNull()
        {
            var go = _util.Track(new GameObject());

            Assert.That(ColorProvider.ResolveTarget(go), Is.Null);
        }

        [Test]
        public void OnEnable_SpriteRenderer_GetsPaletteColor()
        {
            _util.CreateProvider<SpriteRenderer>(_settings, out var sprite);

            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, sprite.color);
        }

        [Test]
        public void OnEnable_ReappliesColor_SoBuildsAndRuntimeAddComponentAreColored()
        {
            // Regression: colors used to be applied only in OnValidate (editor only).
            var provider = _util.CreateProvider<SpriteRenderer>(_settings, out var sprite);

            sprite.color = Color.white;
            provider.gameObject.SetActive(false);
            provider.gameObject.SetActive(true);

            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, sprite.color);
        }

        [Test]
        public void OnEnable_WithoutPalette_AppliesSerializedFinalColor()
        {
            var settingsWithoutPalette = _util.Track(ScriptableObject.CreateInstance<PletSceneSettings>());
            var provider = _util.CreateProvider<SpriteRenderer>(settingsWithoutPalette, out var sprite);

            provider.ColorSettings[0].FinalColor = Color.magenta;
            provider.ApplyColorAction();

            PletTestUtility.AssertColorsEqual(Color.magenta, sprite.color);
        }

        [Test]
        public void ParticleSystem_GetsStartColor()
        {
            // Regression: the ParticleSystemRenderer used to be picked, so the particles were never colored.
            _util.CreateProvider<ParticleSystem>(_settings, out var ps);

            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, ps.main.startColor.color);
        }

        [Test]
        public void Selectable_GetsFiveVisibleSlots()
        {
            // Regression: the four extra slots started at S/V 0 (transparent black) and then made ApplyColor throw.
            var go = _util.Track(new GameObject());
            go.SetActive(false);
            go.AddComponent<Image>();
            var button = go.AddComponent<Button>();
            var provider = go.AddComponent<ColorProvider>();
            provider.UseSceneSettings(_settings);

            Assert.DoesNotThrow(() => go.SetActive(true));
            Assert.That(provider.ColorSettings.Count, Is.EqualTo(ColorProvider.SelectableSlotCount));

            foreach (var setting in provider.ColorSettings)
            {
                Assert.That(setting.Saturation, Is.InRange(1, 19), setting.Name);
                Assert.That(setting.Value, Is.InRange(1, 19), setting.Name);
                Assert.That(setting.FinalColor.a, Is.EqualTo(1f), setting.Name);
            }

            Assert.That(button.colors.disabledColor.a, Is.EqualTo(1f));
            Assert.That(button.colors.disabledColor, Is.Not.EqualTo(Color.black));
        }

        [Test]
        public void Selectable_ExtraSlotsCycleThroughHueTypes()
        {
            var go = _util.Track(new GameObject());
            go.SetActive(false);
            go.AddComponent<Image>();
            go.AddComponent<Button>();
            var provider = go.AddComponent<ColorProvider>();
            provider.UseSceneSettings(_settings);
            go.SetActive(true);

            Assert.That(provider.ColorSettings[0].HueType, Is.EqualTo(HueType.Base));
            Assert.That(provider.ColorSettings[1].HueType, Is.EqualTo(HueType.Tone));
            Assert.That(provider.ColorSettings[2].HueType, Is.EqualTo(HueType.Accent));
            Assert.That(provider.ColorSettings[3].HueType, Is.EqualTo(HueType.Base));
        }

        [Test]
        public void ZeroSaturationSlot_DoesNotThrow()
        {
            var provider = _util.CreateProvider<SpriteRenderer>(_settings, out _);
            provider.ColorSettings[0].Saturation = 0;
            provider.ColorSettings[0].Value = 0;

            Assert.DoesNotThrow(provider.ApplyColorAction);
        }

        [Test]
        public void Camera_ClearFlagsLeftAloneWhenDisabled()
        {
            var go = _util.Track(new GameObject());
            go.SetActive(false);
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            var provider = go.AddComponent<ColorProvider>();
            provider.SetCameraClearFlagsToSolidColor = false;
            provider.UseSceneSettings(_settings);
            go.SetActive(true);

            Assert.That(cam.clearFlags, Is.EqualTo(CameraClearFlags.Skybox));
            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, cam.backgroundColor);
        }

        [Test]
        public void Camera_ClearFlagsSetToSolidColorByDefault()
        {
            _util.CreateProvider<Camera>(_settings, out var cam);

            Assert.That(cam.clearFlags, Is.EqualTo(CameraClearFlags.SolidColor));
        }

        [Test]
        public void Renderer_UsesPropertyBlockAndLeavesMaterialUntouched()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) Assert.Ignore("URP Lit shader not available.");

            var material = _util.Track(new Material(shader));
            var originalColor = material.GetColor(s_baseColorId);

            var go = _util.Track(new GameObject());
            go.SetActive(false);
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            var provider = go.AddComponent<ColorProvider>();
            provider.UseSceneSettings(_settings);
            go.SetActive(true);

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block, 0);

            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, block.GetColor(s_baseColorId));
            Assert.That(material.GetColor(s_baseColorId), Is.EqualTo(originalColor));
        }
    }
}
