using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    [CustomEditor(typeof(TextureProvider))] [CanEditMultipleObjects]
    public class TextureProviderEditor : UnityEditor.Editor
    {
        private SerializedProperty _textureSettingsProp;
        private TextureProvider _textureProvider;


        private void OnEnable()
        {
            _textureProvider = target as TextureProvider;
            _textureSettingsProp = serializedObject.FindProperty(nameof(TextureProvider.TextureSettings));
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            for (var i = 0; i < _textureSettingsProp.arraySize; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                var textureSettingsProp = _textureSettingsProp.GetArrayElementAtIndex(i);
                var materialNameProp = textureSettingsProp.FindPropertyRelative(nameof(TextureSettings.MaterialName));
                var indexProp = textureSettingsProp.FindPropertyRelative(nameof(TextureSettings.Index));
                var previousIndexProp = textureSettingsProp.FindPropertyRelative(nameof(TextureSettings.PreviousIndex));
                var currentTextureProp = textureSettingsProp.FindPropertyRelative(nameof(TextureSettings.CurrentTexture));

                indexProp.intValue = EditorGUILayout.IntSlider(materialNameProp.stringValue, indexProp.intValue, 0, TextureProvider.TextureSaturationSteps - 1);

                if (indexProp.intValue != previousIndexProp.intValue)
                {
                    serializedObject.ApplyModifiedProperties();

                    previousIndexProp.intValue = indexProp.intValue;
                    _textureProvider.GetNextTexture();

                    currentTextureProp = textureSettingsProp.FindPropertyRelative(nameof(TextureSettings.CurrentTexture));
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(currentTextureProp);
                EditorGUI.EndDisabledGroup();

                GUILayout.EndVertical();
            }
        }
    }
}