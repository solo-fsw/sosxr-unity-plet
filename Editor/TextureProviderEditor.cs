using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.EditorScripts
{
    [CustomEditor(typeof(TextureProvider))]
    [CanEditMultipleObjects]
    public class TextureProviderEditor : Editor
    {
        private SerializedProperty _textureSettingsProp;
        private TextureProvider _textureProvider;


        [InitializeOnLoadMethod]
        private static void RegisterDesaturateDelegate()
        {
            TextureProvider.DesaturateTexture = Desaturate.Texture;
        }


        private void OnEnable()
        {
            _textureProvider = target as TextureProvider;

            if (_textureProvider == null)
            {
                Debug.LogError("TextureProviderEditor target is not a TextureProvider.");

                return;
            }

            _textureSettingsProp = serializedObject.FindProperty(nameof(TextureProvider.TextureSettings));
        }


            public override void OnInspectorGUI()
        {
            if (_textureProvider == null || _textureSettingsProp == null)
            {
                EditorGUILayout.HelpBox("TextureProvider is not properly initialized.", MessageType.Error);

                return;
            }

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

                if (currentTextureProp != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(currentTextureProp);
                    EditorGUI.EndDisabledGroup();
                }

                GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}