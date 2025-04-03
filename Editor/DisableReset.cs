using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    /// <summary>
    ///     Disable reset on a class
    ///     as per:    https://discussions.unity.com/t/prevent-reset-from-clearing-out-serialized-fields/191838/4
    /// </summary>
    [ExecuteAlways]
    public static class DisableReset
    {
        [MenuItem("CONTEXT/" + nameof(ColorProvider) + "/Reset", true)]
        private static bool OnValidateReset()
        {
            return false;
        }


        [MenuItem("CONTEXT/" + nameof(ColorProvider) + "/Reset")]
        private static void OnReset()
        {
            Debug.LogWarning("MyScript doesn't support Reset.");
        }
    }
}