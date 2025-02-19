using UnityEditor;
using UnityEngine;


namespace SOSXR.Plet
{
    [CustomPropertyDrawer(typeof(Texture), true)]
    public class TexturePropertyDrawer : PropertyDrawer
    {
        private const float MaxPreviewSize = 500f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            var texture = (Texture) property.objectReferenceValue;
            var previewWidth = MaxPreviewSize;
            var previewHeight = MaxPreviewSize;

            if (texture)
            {
                var aspectRatio = (float) texture.width / texture.height;

                if (aspectRatio > 1)
                {
                    previewHeight = MaxPreviewSize / aspectRatio;
                }
                else
                {
                    previewWidth = MaxPreviewSize * aspectRatio;
                }
            }

            var centerX = position.x + (position.width - previewWidth) / 2;
            var previewRect = new Rect(centerX, position.y + EditorGUIUtility.singleLineHeight + 5, previewWidth, previewHeight);

            if (texture)
            {
                EditorGUI.DrawPreviewTexture(previewRect, texture);
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return MaxPreviewSize + EditorGUIUtility.singleLineHeight + 5;
        }
    }
}