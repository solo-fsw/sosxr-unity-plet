using UnityEngine;


namespace SOSXR.Plet
{
    [CreateAssetMenu(fileName = "Palette", menuName = "SOSXR/Plet/Palette")]
    public class Palette : ScriptableObject
    {
        [Header("Use this around 60% of the time. It is a base. Use a rather neutral color.")]
        public Color Base = new(1, 1, 1, 1);
        [Header("Use this around 30% of the time. This is the 'tone' color.")]
        public Color Tone = new(1, 1, 1, 1);
        [Header("Use this around 10% of the time. This is the 'pop' color.")]
        public Color Accent = new(1, 1, 1, 1);

        [Space(10)]
        [Tooltip("Optional")]
        [SerializeField] private Texture m_optionalColorTexture;

        [HideInInspector] public PaletteHolder PaletteHolder;

        private Color _previousBase;
        private Color _previousTone;
        private Color _previousAccent;


        private void OnValidate()
        {
            RegisterPaletteChanges();
        }


        private void Reset()
        {
            RegisterPaletteChanges();
        }


        public void RegisterPaletteChanges()
        {
            if (_previousBase == Base && _previousTone == Tone && _previousAccent == Accent)
            {
                return;
            }

            _previousBase = Base;
            _previousTone = Tone;
            _previousAccent = Accent;

            PaletteHolder?.OnPaletteChanged?.Invoke();
        }
    }
}