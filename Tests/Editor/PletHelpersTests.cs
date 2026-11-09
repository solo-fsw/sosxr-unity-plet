using NUnit.Framework;

namespace SOSXR.plet.Tests
{
    public class PletHelpersTests
    {
        [TestCase(null, ExpectedResult = PletHelpers.UntitledSceneName)]
        [TestCase("", ExpectedResult = PletHelpers.UntitledSceneName)]
        [TestCase("Level1", ExpectedResult = "Level1")]
        public string GetSafeSceneName_ReturnsFallbackForEmptyNames(string sceneName) => PletHelpers.GetSafeSceneName(sceneName);

        [Test]
        public void TextureFolderPath_IsInsideTheResourcesFolderUsedForLoading()
        {
            // Regression: Desaturate saved to the Resources root while TextureProvider loaded from "Plet/Textures".
            Assert.That(PletHelpers.TextureFolderPath, Is.EqualTo($"{PletHelpers.FolderPath}/{PletHelpers.TextureResourcesFolder}"));
            Assert.That(PletHelpers.TextureFolderPath, Does.Contain("/Resources/"));
        }

        [Test]
        public void TextureResourcePath_PrefixesTheTextureFolder()
        {
            Assert.That(PletHelpers.TextureResourcePath("wood_saturated_50"), Is.EqualTo("Plet/Textures/wood_saturated_50"));
        }

        [Test]
        public void DesaturatedTextureName_UsesSuffixAndPercentage()
        {
            Assert.That(PletHelpers.DesaturatedTextureName("wood", 30), Is.EqualTo("wood_saturated_30"));
        }

        [Test]
        public void SaturationPercentageForStep_CoversZeroToHundred()
        {
            int steps = TextureProvider.TextureSaturationSteps;

            Assert.That(PletHelpers.SaturationPercentageForStep(0, steps), Is.EqualTo(0));
            Assert.That(PletHelpers.SaturationPercentageForStep(steps - 1, steps), Is.EqualTo(100));
            Assert.That(PletHelpers.SaturationPercentageForStep(5, 11), Is.EqualTo(50));
        }

        [Test]
        public void NewColorSettings_StartAtPaletteDefaultNotZero()
        {
            var settings = new ColorSettings();

            Assert.That(settings.Saturation, Is.EqualTo(10));
            Assert.That(settings.Value, Is.EqualTo(10));
            Assert.That(settings.Alpha, Is.EqualTo(1f));
            Assert.That(settings.FinalColor.a, Is.EqualTo(1f));
        }
    }
}
