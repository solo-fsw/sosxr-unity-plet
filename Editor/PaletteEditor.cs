using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    [CustomEditor(typeof(Palette))]
    public class PaletteEditor : UnityEditor.Editor
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