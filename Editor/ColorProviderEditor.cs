using UnityEditor;
using UnityEngine;


namespace SOSXR.Plet
{
    [CustomEditor(typeof(ColorProvider))] [CanEditMultipleObjects]
    public class ColorProviderEditor : PaletteEditorBase
    {
        private SerializedProperty _colorSettingsProp;


        private void OnEnable()
        {
            _colorSettingsProp = serializedObject.FindProperty("m_colorSettings");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            for (var i = 0; i < _colorSettingsProp.arraySize; i++)
            {
                var colorSettingsProp = _colorSettingsProp.GetArrayElementAtIndex(i);
                var nameProp = colorSettingsProp.FindPropertyRelative("Name");
                var colorTypeProp = colorSettingsProp.FindPropertyRelative("ColorType");
                var valueProp = colorSettingsProp.FindPropertyRelative("Value");
                var saturationProp = colorSettingsProp.FindPropertyRelative("Saturation");
                var alphaProp = colorSettingsProp.FindPropertyRelative("Alpha");
                var valuedColorProp = colorSettingsProp.FindPropertyRelative("ValuedColor");

                DrawColorSettings(nameProp, colorTypeProp, valueProp, saturationProp, alphaProp, valuedColorProp);
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawColorSettings(SerializedProperty nameProp, SerializedProperty colorTypeProp, SerializedProperty valueProp, SerializedProperty saturationProp, SerializedProperty alphaProp, SerializedProperty valuedColorProp)
        {
            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            var label = "Color Type";

            if (!string.IsNullOrEmpty(nameProp.stringValue))
            {
                label = nameProp.stringValue;
            }

            DrawProperty(colorTypeProp, "Color Type",
                () => (ColorType) EditorGUILayout.EnumPopup(label, (ColorType) colorTypeProp.enumValueIndex),
                (prop, value) => prop.enumValueIndex = (int) value);

            DrawProperty(valueProp, "Value",
                () => EditorGUILayout.Slider("Value", valueProp.floatValue, ValueRange.x, ValueRange.y),
                (prop, value) => prop.floatValue = value);

            DrawProperty(saturationProp, "Saturation",
                () => EditorGUILayout.Slider("Saturation", saturationProp.floatValue, SaturationRange.x, SaturationRange.y),
                (prop, value) => prop.floatValue = value);

            DrawProperty(alphaProp, "Alpha",
                () => EditorGUILayout.Slider("Alpha", alphaProp.floatValue, 0f, 1f),
                (prop, value) => prop.floatValue = value);

            DrawColorBox(valuedColorProp.colorValue, true);

            GUILayout.EndVertical();
        }


        private void DrawColorBox(Color color, bool alpha = false, bool hdr = false, string label = "Preview")
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField(new GUIContent(label), color, false, alpha, hdr, GUILayout.Height(40));
            EditorGUI.EndDisabledGroup();
        }
    }
}