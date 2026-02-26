using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.EditorScripts
{
    /// <summary>Custom inspector for <see cref="Palette"/> that appends color-theory reference links (W3 Schools analogous, compound, and triadic pickers) below the default fields.</summary>
    [CustomEditor(typeof(Palette))]
    public class PaletteEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            if (EditorGUILayout.LinkButton("W3 Schools - Analogous Color Picker"))
            {
                Application.OpenURL("https://www.w3schools.com/colors/colors_analogous.asp");
            }

            if (EditorGUILayout.LinkButton("W3 Schools - Compound Color Picker"))
            {
                Application.OpenURL("https://www.w3schools.com/colors/colors_compound.asp");
            }

            if (EditorGUILayout.LinkButton("W3 Schools - Triadic Color Picker"))
            {
                Application.OpenURL("https://www.w3schools.com/colors/colors_triadic.asp");
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox("On the above sites, the middle three of the five colors correspond with our use of base / tone / accent, but not necessarily in that order.", MessageType.Info);
        }
    }
}