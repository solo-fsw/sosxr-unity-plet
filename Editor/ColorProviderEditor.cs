using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    [CustomEditor(typeof(ColorProvider))] [CanEditMultipleObjects]
    public class ColorProviderEditor : PaletteEditorBase
    {
        private SerializedProperty _colorSettingsProp;


        private void OnEnable()
        {
            _colorSettingsProp = serializedObject.FindProperty(nameof(ColorProvider.ColorSettings));
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            for (var i = 0; i < _colorSettingsProp.arraySize; i++)
            {
                var colorSettingsProp = _colorSettingsProp.GetArrayElementAtIndex(i);
                var nameProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.Name));
                var colorTypeProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.HueType));
                var valueProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.Value));
                var saturationProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.Saturation));
                var alphaProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.Alpha));
                var useAlphaProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.ShowAlpha));
                var valuedColorProp = colorSettingsProp.FindPropertyRelative(nameof(ColorSettings.FinalColor));

                DrawSection(nameProp.stringValue, colorTypeProp, valueProp, saturationProp, valuedColorProp, alphaProp, useAlphaProp.boolValue);

                EditorGUILayout.Space(10);
            }


            if (GUILayout.Button(ButtonText))
            {
                foreach (var colorProviderObj in targets)
                {
                    ((ColorProvider) colorProviderObj).GetPaletteSaturationAndValue();
                }

                SceneView.RepaintAll();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}