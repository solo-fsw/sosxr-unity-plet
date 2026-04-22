using UnityEngine;

namespace SOSXR.plet
{
    /// <summary>
    ///     ScriptableObject asset representing a three-color palette following the 60/30/10 rule.
    ///     Holds a <see cref="Base"/> (dominant, ~60%), <see cref="Tone"/> (secondary, ~30%),
    ///     and <see cref="Accent"/> (~10%) color, plus an optional reference texture for design reference.
    /// </summary>
    [CreateAssetMenu(fileName = "Palette", menuName = "SOSXR/plet/Palette")]
    public sealed class Palette : ScriptableObject
    {
        /// <summary>Dominant (~60%) palette color; generally a neutral base.</summary>
        [Header("Use this around 60% of the time. It is a base. Use a rather neutral color.")]
        public Color Base = new(1, 1, 1, 1);

        /// <summary>Secondary (~30%) palette color used to support the base.</summary>
        [Header("Use this around 30% of the time. This is the 'tone' color.")]
        public Color Tone = new(1, 1, 1, 1);

        /// <summary>Accent (~10%) palette color used sparingly for emphasis.</summary>
        [Header("Use this around 10% of the time. This is the 'pop' color.")]
        public Color Accent = new(1, 1, 1, 1);

        /// <summary>Optional reference texture (e.g., a moodboard or screenshot) used when authoring the palette.</summary>
        [Space(10)]
        [Tooltip("Optional")]
        [TexturePreview(500)]
        [SerializeField]
        private Texture _optionalColorTexture;
    }
}
