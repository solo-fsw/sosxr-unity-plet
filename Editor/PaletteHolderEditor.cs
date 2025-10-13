using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SOSXR.plet.EditorScripts
{
    [CustomEditor(typeof(PletSceneSettings))]
    public class PaletteHolderEditor : PaletteEditorBase
    {
        private SerializedProperty _paletteProp;
        private SerializedProperty _previousPaletteProp;

        private SerializedProperty _applySkyboxProp;

        private SerializedProperty _skyboxMaterialProp;

        private SerializedProperty _skyboxSkyHueTypeProp;
        private SerializedProperty _skyboxSkySaturationProp;
        private SerializedProperty _skyboxSkyValueProp;
        private SerializedProperty _skyboxSkyColorProp;

        private SerializedProperty _skyboxHorizonHueTypeProp;
        private SerializedProperty _skyboxHorizonSaturationProp;
        private SerializedProperty _skyboxHorizonValueProp;
        private SerializedProperty _skyboxHorizonColorProp;

        private SerializedProperty _skyboxGroundHueTypeProp;
        private SerializedProperty _skyboxGroundSaturationProp;
        private SerializedProperty _skyboxGroundValueProp;
        private SerializedProperty _skyboxGroundColorProp;

        private SerializedProperty _applyAmbientLightProp;

        private SerializedProperty _ambientLightHueTypeProp;
        private SerializedProperty _ambientLightSaturationProp;
        private SerializedProperty _ambientLightValueProp;
        private SerializedProperty _ambientLightColorProp;

        private SerializedProperty _ambientSkyLightTypeProp;
        private SerializedProperty _ambientSkyLightSaturationProp;
        private SerializedProperty _ambientSkyLightValueProp;
        private SerializedProperty _ambientSkyLightColorProp;

        private SerializedProperty _ambientEquatorLightTypeProp;
        private SerializedProperty _ambientEquatorLightSaturationProp;
        private SerializedProperty _ambientEquatorLightValueProp;
        private SerializedProperty _ambientEquatorLightColorProp;

        private SerializedProperty _ambientGroundLightTypeProp;
        private SerializedProperty _ambientGroundLightSaturationProp;
        private SerializedProperty _ambientGroundLightValueProp;
        private SerializedProperty _ambientGroundLightColorProp;

        private SerializedProperty _applyRealtimeShadowsProp;

        private SerializedProperty _realtimeShadowHueTypeProp;
        private SerializedProperty _realtimeShadowSaturationProp;
        private SerializedProperty _realtimeShadowValueProp;
        private SerializedProperty _realtimeShadowColorProp;

        private SerializedProperty _applyFogProp;

        private SerializedProperty _fogHueTypeProp;
        private SerializedProperty _fogSaturationProp;
        private SerializedProperty _fogValueProp;
        private SerializedProperty _fogColorProp;


        private void OnEnable()
        {
            _paletteProp = serializedObject.FindProperty(nameof(PletSceneSettings.Palette));
            _previousPaletteProp = serializedObject.FindProperty(nameof(PletSceneSettings.PreviousPalette));

            _applySkyboxProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplySkybox));

            _skyboxMaterialProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxMaterial));
            _skyboxSkyHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxSkyHueType));
            _skyboxSkySaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxSkySaturation));
            _skyboxSkyValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxSkyValue));
            _skyboxSkyColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxSkyColor));

            _skyboxHorizonHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxHorizonHueType));
            _skyboxHorizonSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxHorizonSaturation));
            _skyboxHorizonValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxHorizonValue));
            _skyboxHorizonColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxHorizonColor));

            _skyboxGroundHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxGroundHueType));
            _skyboxGroundSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxGroundSaturation));
            _skyboxGroundValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxGroundValue));
            _skyboxGroundColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxGroundColor));

            _applyAmbientLightProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplyAmbientLight));

            _ambientLightHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientLightHueType));
            _ambientLightSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientLightSaturation));
            _ambientLightValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientLightValue));
            _ambientLightColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientLightColor));

            _ambientSkyLightTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientSkyLightHueType));
            _ambientSkyLightSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientSkyLightSaturation));
            _ambientSkyLightValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientSkyLightValue));
            _ambientSkyLightColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientSkyLightColor));

            _ambientEquatorLightTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientEquatorLightHueType));
            _ambientEquatorLightSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientEquatorLightSaturation));
            _ambientEquatorLightValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientEquatorLightValue));
            _ambientEquatorLightColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientEquatorLightColor));

            _ambientGroundLightTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientGroundLightHueType));
            _ambientGroundLightSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientGroundLightSaturation));
            _ambientGroundLightValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientGroundLightValue));
            _ambientGroundLightColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.AmbientGroundLightColor));

            _applyRealtimeShadowsProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplyRealtimeShadows));

            _realtimeShadowHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowHueType));
            _realtimeShadowSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowSaturation));
            _realtimeShadowValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowValue));
            _realtimeShadowColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowColor));

            _applyFogProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplyFog));

            _fogHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogHueType));
            _fogSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogSaturation));
            _fogValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogValue));
            _fogColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogColor));
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var paletteHolder = (PletSceneSettings)target;

            if (!PletSceneSettings.UseThisPaletteSceneSettings())
            {
                EditorGUILayout.HelpBox("This Palette Holder is not being used in the scene. It will not apply any colors. If you thought it should have worked in this scene, please check the following:\n" +
                                        "1. It is either the only PaletteHolder in any of the Resources folders\n" +
                                        "2. It is one of multiple PaletteHolder in any Resource folder, but it has the (exact) name of the current scene\n" +
                                        "3. (In case of 2) It is the only PaletteHolder with the name of the scene\n" +
                                        "4. You are currently in the scene where this PaletteHolder belongs to.", MessageType.Warning);

                return;
            }

            if (!HasPalette())
            {
                return;
            }

            var palette = (Palette)_paletteProp.objectReferenceValue;

            if (palette != _previousPaletteProp.objectReferenceValue)
            {
                _paletteProp.objectReferenceValue = palette;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            DrawSectionHeader("Palette Colors");
            DrawPaletteColorFields((Palette)_paletteProp.objectReferenceValue);

            EditorGUILayout.Space(10);

            if (DrawToggleHeader("Skybox", "Apply Skybox", _applySkyboxProp))
            {
                DrawSkyboxField(paletteHolder);

                if (_skyboxMaterialProp.objectReferenceValue != null)
                {
                    DrawSkyboxColorFields(paletteHolder);
                }
                else
                {
                    EditorGUILayout.Space(10);

                    EditorGUILayout.HelpBox("Please assign a Skybox Material to apply colors.", MessageType.Warning);
                }
            }


            if (RenderSettings.ambientMode != AmbientMode.Skybox)
            {
                EditorGUILayout.Space(10);

                if (DrawToggleHeader("Ambient Light", "Apply Ambient Light Colors", _applyAmbientLightProp))
                {
                    if (RenderSettings.ambientMode == AmbientMode.Flat)
                    {
                        DrawSingleAmbientLightFields(paletteHolder);
                    }
                    else if (RenderSettings.ambientMode == AmbientMode.Trilight)
                    {
                        DrawTriAmbientLightFields(paletteHolder);
                    }
                }
            }
            else
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Ambient Light is disabled because the Environment Lighting is set to Skybox in the Lighting - Environment", MessageType.Info);
            }

            EditorGUILayout.Space(10);

            if (DrawToggleHeader("Realtime Shadows", "Apply Realtime Shadow Color", _applyRealtimeShadowsProp))
            {
                DrawRealtimeShadowFields(paletteHolder);
            }


            if (RenderSettings.fog)
            {
                EditorGUILayout.Space(10);

                if (DrawToggleHeader("Fog", "Apply Fog Color", _applyFogProp))
                {
                    DrawFogFields(paletteHolder);
                }
            }
            else
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.HelpBox("Fog is disabled in the Lighting - Environment", MessageType.Info);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                Repaint();
            }
        }


        /// <summary>
        ///     Draws the palette selector and handles palette changes.
        /// </summary>
        /// <returns>True if a valid palette is selected</returns>
        private bool HasPalette()
        {
            EditorGUILayout.PropertyField(_paletteProp);

            if (_paletteProp.objectReferenceValue == null)
            {
                return false;
            }

            var palette = (Palette)_paletteProp.objectReferenceValue;

            if (palette != _previousPaletteProp.objectReferenceValue)
            {
                _paletteProp.objectReferenceValue = palette;
                serializedObject.ApplyModifiedProperties();
            }

            return true;
        }


        /// <summary>
        ///     Draws a header with a toggle and returns the toggle state.
        /// </summary>
        /// <returns>Current toggle value</returns>
        private bool DrawToggleHeader(string label, string toggleLabel, SerializedProperty toggleProp)
        {
            EditorGUILayout.BeginHorizontal();
            DrawSectionHeader(label);
            toggleProp.boolValue = EditorGUILayout.ToggleLeft(toggleLabel, toggleProp.boolValue);
            EditorGUILayout.EndHorizontal();

            return toggleProp.boolValue;
        }


        /// <summary>
        ///     Draws a section header with bold styling.
        /// </summary>
        private void DrawSectionHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }


        private void DrawPaletteColorFields(Palette palette)
        {
            var height = 50;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Base, false, false, nameof(HueType) + " - " + nameof(HueType.Base), height);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Tone, false, false, nameof(HueType) + " - " + nameof(HueType.Tone), height);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Accent, false, false, nameof(HueType) + " - " + nameof(HueType.Accent), height);
            EditorGUILayout.EndVertical();
        }


        private void DrawSkyboxField(PletSceneSettings pletSceneSettings)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            DrawProperty(_skyboxMaterialProp, "Skybox Material",
                () => _skyboxMaterialProp.objectReferenceValue = (Material)EditorGUILayout.ObjectField("Skybox Material", _skyboxMaterialProp.objectReferenceValue, typeof(Material), false),
                (prop, newValue) => prop.objectReferenceValue = newValue);

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetSkyboxMaterial();
            }
        }


        private void DrawSkyboxColorFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Skybox Sky Hue Type", _skyboxSkyHueTypeProp, _skyboxSkyValueProp, _skyboxSkySaturationProp, _skyboxSkyColorProp);
            DrawSection("Skybox Horizon Hue Type", _skyboxHorizonHueTypeProp, _skyboxHorizonValueProp, _skyboxHorizonSaturationProp, _skyboxHorizonColorProp);
            DrawSection("Skybox Ground Hue Type", _skyboxGroundHueTypeProp, _skyboxGroundValueProp, _skyboxGroundSaturationProp, _skyboxGroundColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetSkyboxSkyColor();
                pletSceneSettings.SetSkyboxHorizonColor();
                pletSceneSettings.SetSkyboxGroundColor();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetSkyboxSVFromPalette();
            }
        }


        private void DrawSingleAmbientLightFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Ambient Light Hue Type", _ambientLightHueTypeProp, _ambientLightValueProp, _ambientLightSaturationProp, _ambientLightColorProp, null, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetAmbientLightColor();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetAmbientLightSVFromPalette();
            }
        }


        private void DrawTriAmbientLightFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Ambient Sky Light Hue Type", _ambientSkyLightTypeProp, _ambientSkyLightValueProp, _ambientSkyLightSaturationProp, _ambientSkyLightColorProp, null, true);
            DrawSection("Ambient Equator Light Hue Type", _ambientEquatorLightTypeProp, _ambientEquatorLightValueProp, _ambientEquatorLightSaturationProp, _ambientEquatorLightColorProp, null, true);
            DrawSection("Ambient Ground Light Hue Type", _ambientGroundLightTypeProp, _ambientGroundLightValueProp, _ambientGroundLightSaturationProp, _ambientGroundLightColorProp, null, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                pletSceneSettings.SetAmbientSkyTriLightColor();
                pletSceneSettings.SetAmbientEquatorTriLightColor();
                pletSceneSettings.SetAmbientGroundTriLightColor();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetAmbientLightSVFromPalette();
            }
        }


        private void DrawRealtimeShadowFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Realtime Shadow Hue Type", _realtimeShadowHueTypeProp, _realtimeShadowValueProp, _realtimeShadowSaturationProp, _realtimeShadowColorProp);


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetRealtimeShadowColor();
            }

            if (GUILayout.Button(nameof(pletSceneSettings.GetRealtimeShadowSVFromPalette)))
            {
                pletSceneSettings.GetRealtimeShadowSVFromPalette();
            }
        }


        private void DrawFogFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Fog Hue Type", _fogHueTypeProp, _fogValueProp, _fogSaturationProp, _fogColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetFogColor();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetFogSVFromPalette();
            }
        }
    }
}