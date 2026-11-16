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
        public void DesaturatedTextureBaseName_AddsGuidPrefix_SoSameNamedTexturesDoNotCollide()
        {
            // Regression: variants were keyed by texture name only, in one flat folder.
            string a = PletHelpers.DesaturatedTextureBaseName("wood", "1a2b3c4d5e6f77889900aabbccddeeff");
            string b = PletHelpers.DesaturatedTextureBaseName("wood", "ffeeddccbbaa00998877f6e5d4c3b2a1");

            Assert.That(a, Is.EqualTo("wood_1a2b3c4d"));
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [TestCase(null)]
        [TestCase("")]
        public void DesaturatedTextureBaseName_WithoutGuid_ReturnsTextureName(string guid)
        {
            Assert.That(PletHelpers.DesaturatedTextureBaseName("wood", guid), Is.EqualTo("wood"));
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
