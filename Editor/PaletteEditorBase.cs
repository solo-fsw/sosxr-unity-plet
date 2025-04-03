using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    public class PaletteEditorBase : UnityEditor.Editor
    {
        protected const string ButtonText = "Get Palette Saturation and Value";


        /// <summary>
        ///     Draw a section for color properties.
        /// </summary>
        protected void DrawSection(string label, SerializedProperty typeProp, SerializedProperty valueProp, SerializedProperty saturationProp, SerializedProperty colorProp, SerializedProperty alphaProp = null, bool useAlpha = false, bool useHDR = false)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (string.IsNullOrEmpty(label))
            {
                label = "Hue Type";
            }

            DrawProperty(typeProp, label,
                () => (HueType) EditorGUILayout.EnumPopup(label, (HueType) typeProp.enumValueIndex),
                (prop, newValue) => prop.enumValueIndex = (int) newValue);

            DrawProperty(saturationProp, "Saturation",
                () => EditorGUILayout.IntSlider("Saturation", saturationProp.intValue, Saturation.DisplayRange.x, Saturation.DisplayRange.y),
                (prop, newValue) => prop.intValue = newValue);

            DrawProperty(valueProp, "Value",
                () => EditorGUILayout.IntSlider("Value", valueProp.intValue, Value.DisplayRange.x, Value.DisplayRange.y),
                (prop, newValue) => prop.intValue = newValue);

            if (alphaProp != null && useAlpha)
            {
                DrawProperty(alphaProp, "Alpha",
                    () => EditorGUILayout.Slider("Alpha", alphaProp.floatValue, 0f, 1f),
                    (prop, value) => prop.floatValue = value);
            }

            DrawColorBox(colorProp.colorValue, alphaProp != null && useAlpha, useHDR, useHDR ? "HDR Preview" : "Preview");

            GUILayout.EndVertical();
        }


        /// <summary>
        ///     Generic method to handle property changes safely.
        /// </summary>
        protected void DrawProperty<T>(SerializedProperty property, string undoName, Func<T> drawField, Action<SerializedProperty, T> applyChange)
        {
            EditorGUI.BeginChangeCheck();
            var newValue = drawField();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(serializedObject.targetObjects, $"Change {undoName}");

                foreach (var targetObject in serializedObject.targetObjects)
                {
                    var so = new SerializedObject(targetObject);
                    var prop = so.FindProperty(property.propertyPath); // Get the correct property path
                    applyChange(prop, newValue);
                    so.ApplyModifiedProperties();
                }
            }
        }


        protected static void DrawColorBox(Color color, bool alpha = false, bool hdr = false, string label = "Preview", int height = 35)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField(new GUIContent(label), color, false, alpha, hdr, GUILayout.Height(height));
            EditorGUI.EndDisabledGroup();
        }
    }
}