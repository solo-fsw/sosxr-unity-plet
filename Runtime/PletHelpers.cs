using UnityEngine;
using UnityEngine.SceneManagement;

namespace SOSXR.plet
{
    public static class PletHelpers
    {
        public static PletSceneSettings GetPletSceneSettings()
        {
            var paletteHolders = Resources.LoadAll<PletSceneSettings>("");

            if (paletteHolders.Length == 1)
            {
                return paletteHolders[0];
            }

            if (paletteHolders.Length == 0)
            {
                Debug.LogWarning("No PaletteSceneSettings found in any of the Resources folders.");

                return null;
            }

            var activeScene = SceneManager.GetActiveScene();

            foreach (var paletteHolder in paletteHolders)
            {
                if (paletteHolder.name == activeScene.name)
                {
                    return paletteHolder;
                }
            }

            Debug.LogWarningFormat("Multiple PaletteSettings found in Resources folders, but none with the same name as the scene: {0}", activeScene.name);

            return null;
        }
    }
}