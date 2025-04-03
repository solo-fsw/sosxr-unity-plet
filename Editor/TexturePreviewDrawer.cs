using UnityEditor;
using UnityEngine;


namespace SOSXR.plet.Editor
{
    [CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
    public class TexturePreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attr = (TexturePreviewAttribute) attribute;
            var maxSize = (float) attr.MaxSize;

            var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            var texture = (Texture) property.objectReferenceValue;

            if (texture)
            {
                var aspectRatio = (float) texture.width / texture.height;
                var previewWidth = maxSize;
                var previewHeight = maxSize;

                if (aspectRatio > 1)
                {
                    previewHeight = maxSize / aspectRatio;
                }
                else
                {
                    previewWidth = maxSize * aspectRatio;
                }

                var centerX = position.x + (position.width - previewWidth) / 2;
                var previewRect = new Rect(centerX, position.y + EditorGUIUtility.singleLineHeight + 5, previewWidth, previewHeight);

                EditorGUI.DrawPreviewTexture(previewRect, texture);
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (TexturePreviewAttribute) attribute;

            return attr.MaxSize + EditorGUIUtility.singleLineHeight + 5;
        }
    }
}