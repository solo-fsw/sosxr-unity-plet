using NUnit.Framework;
using UnityEngine;

namespace SOSXR.plet.Tests
{
    public class PletSceneSettingsTests
    {
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
        public void ApplyColor_AtPaletteSaturationAndValue_ReturnsPaletteColor()
        {
            var sv = _settings.GetColorSV(HueType.Base);
            var color = _settings.ApplyColor(HueType.Base, sv.x, sv.y);

            PletTestUtility.AssertColorsEqual(PletTestUtility.BaseColor, color);
        }

        [Test]
        public void ApplyColor_ForUnsaturatedPaletteColor_IsAbsoluteAndMatchesPaletteAtItsOwnSV()
        {
            // Saturation/Value are absolute HSV on the 1–19 scale, not relative to the palette colour.
            var paletteColor = Color.HSVToRGB(0.6f, 0.3f, 0.8f);
            _settings.Palette.Tone = paletteColor;

            var sv = _settings.GetColorSV(HueType.Tone);
            Assert.That(sv, Is.Not.EqualTo(new Vector2Int(10, 10)));

            // One display step is ~0.055 HSV, so allow for rounding to the nearest step.
            PletTestUtility.AssertColorsEqual(paletteColor, _settings.ApplyColor(HueType.Tone, sv.x, sv.y), 0.06f);

            Color.RGBToHSV(_settings.ApplyColor(HueType.Tone, 10, 10), out _, out float s, out float v);
            Assert.That(s, Is.EqualTo(0.50375f).Within(0.001f));
            Assert.That(v, Is.EqualTo(0.50375f).Within(0.001f));
        }

        [TestCase(HueType.Base)]
        [TestCase(HueType.Tone)]
        [TestCase(HueType.Accent)]
        public void ApplyColor_KeepsPaletteHue(HueType hueType)
        {
            var color = _settings.ApplyColor(hueType, 10, 10);
            Color.RGBToHSV(color, out float hue, out _, out _);

            var expected = hueType switch
            {
                HueType.Base => PletTestUtility.BaseColor,
                HueType.Tone => PletTestUtility.ToneColor,
                _ => PletTestUtility.AccentColor
            };
            Color.RGBToHSV(expected, out float expectedHue, out _, out _);

            Assert.That(hue, Is.EqualTo(expectedHue).Within(0.001f));
        }

        [Test]
        public void ApplyColor_ZeroSaturationAndValue_DoesNotThrowAndClampsToMinimum()
        {
            // Regression: new ColorSettings used to start at S/V 0, which made ApplyColor throw.
            Color color = default;
            Assert.DoesNotThrow(() => color = _settings.ApplyColor(HueType.Base, 0, 0));

            PletTestUtility.AssertColorsEqual(_settings.ApplyColor(HueType.Base, 1, 1), color);
        }

        [Test]
        public void ApplyColor_AboveRange_ClampsToMaximum()
        {
            PletTestUtility.AssertColorsEqual(_settings.ApplyColor(HueType.Tone, 19, 19), _settings.ApplyColor(HueType.Tone, 50, 50));
        }

        [Test]
        public void ApplyColor_ClampsAlpha()
        {
            Assert.That(_settings.ApplyColor(HueType.Base, 10, 10, 2f).a, Is.EqualTo(1f));
            Assert.That(_settings.ApplyColor(HueType.Base, 10, 10, -1f).a, Is.EqualTo(0f));
        }

        [Test]
        public void GetColorSV_ForFullySaturatedColor_ReturnsMaximum()
        {
            Assert.That(_settings.GetColorSV(HueType.Base), Is.EqualTo(new Vector2Int(19, 19)));
        }

        [Test]
        public void GetColorSV_ForBlack_ReturnsValueInDisplayRange()
        {
            _settings.Palette.Accent = Color.black;
            var sv = _settings.GetColorSV(HueType.Accent);

            Assert.That(sv.x, Is.InRange(1, 19));
            Assert.That(sv.y, Is.InRange(1, 19));
        }

        [Test]
        public void GetSkyboxMaterialName_ForUntitledScene_IsNotEmptyPrefix()
        {
            // Regression: an untitled scene produced "_plet_skybox".
            Assert.That(PletSceneSettings.GetSkyboxMaterialName(""), Is.EqualTo("Untitled_plet_skybox"));
            Assert.That(PletSceneSettings.GetSkyboxMaterialName("MyScene"), Is.EqualTo("MyScene_plet_skybox"));
        }

        [TestCase("MyScene_plet_skybox", ExpectedResult = true)]
        [TestCase("Untitled_plet_skybox", ExpectedResult = true)]
        [TestCase("MyOwnSkybox", ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(null, ExpectedResult = false)]
        public bool IsGeneratedSkyboxMaterialName_OnlyMatchesGeneratedNames(string materialName) =>
            // A user-assigned skybox material (any other name) is used as is and never replaced.
            PletSceneSettings.IsGeneratedSkyboxMaterialName(materialName);
    }
}
