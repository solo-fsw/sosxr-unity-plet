using UnityEngine;


namespace SOSXR.plet
{
    public class pletHelpers
    {
        private PaletteHolder _paletteHolder;


        public PaletteHolder GetPaletteHolder(GameObject caller)
        {
            if (_paletteHolder != null)
            {
                return _paletteHolder;
            }

            var paletteHolders = Resources.LoadAll<PaletteHolder>("");

            if (paletteHolders.Length == 1)
            {
                return paletteHolders[0];
            }

            if (paletteHolders.Length == 0)
            {
                Debug.LogWarning("No PaletteHolder found in any of the Resources folders.");

                return null;
            }

            foreach (var paletteHolder in paletteHolders)
            {
                if (paletteHolder.name == caller.scene.name)
                {
                    return paletteHolder;
                }
            }

            Debug.LogWarningFormat("Multiple PaletteHolders found in Resources folders, but none with the same name as the scene: {0}", caller.scene.name);

            return null;
        }
    }
}