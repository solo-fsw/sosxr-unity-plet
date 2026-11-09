using NUnit.Framework;
using UnityEngine;

namespace SOSXR.plet.Tests
{
    public class DesaturateTests
    {
        private Texture2D _source;

        [SetUp]
        public void SetUp()
        {
            _source = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _source.SetPixel(0, 0, new Color(1f, 0f, 0f, 0.5f));
            _source.Apply();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_source);

        [Test]
        public void CalculateDesaturation_Zero_GivesLuminanceGreyAndKeepsAlpha()
        {
            var result = Desaturate.CalculateDesaturation(_source, 0f);
            var pixel = result.GetPixel(0, 0);
            Object.DestroyImmediate(result);

            Assert.That(pixel.r, Is.EqualTo(0.3f).Within(0.01f));
            Assert.That(pixel.g, Is.EqualTo(0.3f).Within(0.01f));
            Assert.That(pixel.b, Is.EqualTo(0.3f).Within(0.01f));
            Assert.That(pixel.a, Is.EqualTo(0.5f).Within(0.01f));
        }

        [Test]
        public void CalculateDesaturation_One_KeepsSourceColor()
        {
            var result = Desaturate.CalculateDesaturation(_source, 1f);
            var pixel = result.GetPixel(0, 0);
            Object.DestroyImmediate(result);

            PletTestUtility.AssertColorsEqual(new Color(1f, 0f, 0f, 0.5f), pixel);
        }

        [Test]
        public void Texture_NullSource_ReturnsNull()
        {
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "No texture provided.");

            Assert.That(Desaturate.Texture(null, 50), Is.Null);
        }
    }
}
