using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.Plet
{
    public class PaletteEditorBase : Editor
    {
        protected static readonly Vector2Int ValueRange = new(1, 19);
        protected static readonly Vector2Int SaturationRange = new(1, 19);


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
    }
}