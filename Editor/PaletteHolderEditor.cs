using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace SOSXR.Plet
{
    [CustomEditor(typeof(PaletteHolder))]
    public class PaletteHolderEditor : PaletteEditorBase
    {
        private SerializedProperty _paletteProp;
        private SerializedProperty _previousPaletteProp;

        private SerializedProperty _applySkyboxProp;

        private SerializedProperty _skyboxMaterialProp;
        private SerializedProperty _skyColorTypeProp;
        private SerializedProperty _skyColorValueProp;
        private SerializedProperty _skyColorSaturationProp;
        private SerializedProperty _skyColorProp;

        private SerializedProperty _horizonColorTypeProp;
        private SerializedProperty _horizonColorValueProp;
        private SerializedProperty _horizonColorSaturationProp;
        private SerializedProperty _horizonColorProp;

        private SerializedProperty _groundColorTypeProp;
        private SerializedProperty _groundColorValueProp;
        private SerializedProperty _groundColorSaturationProp;
        private SerializedProperty _groundColorProp;

        private SerializedProperty _applyAmbientLightProp;

        private SerializedProperty _ambientLightTypeProp;
        private SerializedProperty _ambientLightValueProp;
        private SerializedProperty _ambientLightSaturationProp;
        private SerializedProperty _ambientLightColorProp;

        private SerializedProperty _ambientSkyLightTypeProp;
        private SerializedProperty _ambientSkyLightValueProp;
        private SerializedProperty _ambientSkyLightSaturationProp;
        private SerializedProperty _ambientSkyLightColorProp;

        private SerializedProperty _ambientEquatorLightTypeProp;
        private SerializedProperty _ambientEquatorLightValueProp;
        private SerializedProperty _ambientEquatorLightSaturationProp;
        private SerializedProperty _ambientEquatorLightColorProp;

        private SerializedProperty _ambientGroundLightTypeProp;
        private SerializedProperty _ambientGroundLightValueProp;
        private SerializedProperty _ambientGroundLightSaturationProp;
        private SerializedProperty _ambientGroundLightColorProp;

        private SerializedProperty _applyRealtimeShadowsProp;
        private SerializedProperty _shadowColorTypeProp;
        private SerializedProperty _shadowColorValueProp;
        private SerializedProperty _shadowColorSaturationProp;
        private SerializedProperty _shadowColorProp;

        private SerializedProperty _applyFogProp;
        private SerializedProperty _fogColorTypeProp;
        private SerializedProperty _fogColorValueProp;
        private SerializedProperty _fogColorSaturationProp;
        private SerializedProperty _fogColorProp;

        private Material skybox;


        private void OnEnable()
        {
            _paletteProp = serializedObject.FindProperty("m_palette");
            _previousPaletteProp = serializedObject.FindProperty("m_previousPalette");

            _applySkyboxProp = serializedObject.FindProperty("m_applySkybox");
            _skyboxMaterialProp = serializedObject.FindProperty("m_skyboxMaterial");
            _skyColorTypeProp = serializedObject.FindProperty("m_skyColorType");
            _skyColorValueProp = serializedObject.FindProperty("m_skyColorValue");
            _skyColorSaturationProp = serializedObject.FindProperty("m_skyColorSaturation");
            _skyColorProp = serializedObject.FindProperty("m_skyColor");

            _horizonColorTypeProp = serializedObject.FindProperty("m_horizonColorType");
            _horizonColorValueProp = serializedObject.FindProperty("m_horizonColorValue");
            _horizonColorSaturationProp = serializedObject.FindProperty("m_horizonColorSaturation");
            _horizonColorProp = serializedObject.FindProperty("m_horizonColor");

            _groundColorTypeProp = serializedObject.FindProperty("m_groundColorType");
            _groundColorValueProp = serializedObject.FindProperty("m_groundColorValue");
            _groundColorSaturationProp = serializedObject.FindProperty("m_groundColorSaturation");
            _groundColorProp = serializedObject.FindProperty("m_groundColor");

            _applyAmbientLightProp = serializedObject.FindProperty("m_applyAmbientLight");

            _ambientLightTypeProp = serializedObject.FindProperty("m_ambientLightType");
            _ambientLightValueProp = serializedObject.FindProperty("m_ambientLightValue");
            _ambientLightSaturationProp = serializedObject.FindProperty("m_ambientLightSaturation");
            _ambientLightColorProp = serializedObject.FindProperty("m_ambientLightColor");

            _ambientSkyLightTypeProp = serializedObject.FindProperty("m_ambientSkyLightType");
            _ambientSkyLightValueProp = serializedObject.FindProperty("m_ambientSkyLightValue");
            _ambientSkyLightSaturationProp = serializedObject.FindProperty("m_ambientSkyLightSaturation");
            _ambientSkyLightColorProp = serializedObject.FindProperty("m_ambientSkyLightColor");

            _ambientEquatorLightTypeProp = serializedObject.FindProperty("m_ambientEquatorLightType");
            _ambientEquatorLightValueProp = serializedObject.FindProperty("m_ambientEquatorLightValue");
            _ambientEquatorLightSaturationProp = serializedObject.FindProperty("m_ambientEquatorLightSaturation");
            _ambientEquatorLightColorProp = serializedObject.FindProperty("m_ambientEquatorLightColor");

            _ambientGroundLightTypeProp = serializedObject.FindProperty("m_ambientGroundLightType");
            _ambientGroundLightValueProp = serializedObject.FindProperty("m_ambientGroundLightValue");
            _ambientGroundLightSaturationProp = serializedObject.FindProperty("m_ambientGroundLightSaturation");
            _ambientGroundLightColorProp = serializedObject.FindProperty("m_ambientGroundLightColor");

            _applyRealtimeShadowsProp = serializedObject.FindProperty("m_applyRealtimeShadows");
            _shadowColorTypeProp = serializedObject.FindProperty("m_shadowColorType");
            _shadowColorValueProp = serializedObject.FindProperty("m_shadowColorValue");
            _shadowColorSaturationProp = serializedObject.FindProperty("m_shadowColorSaturation");
            _shadowColorProp = serializedObject.FindProperty("m_shadowColor");

            _applyFogProp = serializedObject.FindProperty("m_applyFog");
            _fogColorTypeProp = serializedObject.FindProperty("m_fogColorType");
            _fogColorValueProp = serializedObject.FindProperty("m_fogColorValue");
            _fogColorSaturationProp = serializedObject.FindProperty("m_fogColorSaturation");
            _fogColorProp = serializedObject.FindProperty("m_fogColor");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var paletteHolder = (PaletteHolder) target;

            if (!paletteHolder.UseThisPaletteHolder())
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

            var palette = (Palette) _paletteProp.objectReferenceValue;

            if (palette != _previousPaletteProp.objectReferenceValue)
            {
                _paletteProp.objectReferenceValue = palette;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            DrawSectionHeader("Palette Colors");
            DrawPaletteColorFields((Palette) _paletteProp.objectReferenceValue);

            EditorGUILayout.Space(10);

            if (DrawToggleHeader("Skybox", "Apply Skybox", _applySkyboxProp))
            {
                DrawSkyboxField(paletteHolder);

                if (skybox != null)
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

            if (DrawToggleHeader("Realtime Shadows", "Apply Realtime Shadow Colors", _applyRealtimeShadowsProp))
            {
                DrawRealtimeShadowFields(paletteHolder);
            }


            if (RenderSettings.fog)
            {
                EditorGUILayout.Space(10);

                if (DrawToggleHeader("Fog", "Apply Fog Colors", _applyFogProp))
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

            var palette = (Palette) _paletteProp.objectReferenceValue;

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
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(nameof(ColorType.Base), EditorStyles.boldLabel);
            DrawColorBox(palette.Base, false, false, null);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(nameof(ColorType.Tone), EditorStyles.boldLabel);
            DrawColorBox(palette.Tone, false, false, null);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(nameof(ColorType.Accent), EditorStyles.boldLabel);
            DrawColorBox(palette.Accent, false, false, null);
            EditorGUILayout.EndVertical();
        }


        private void DrawColorBox(Color color, bool alpha = false, bool hdr = false, string label = "Preview")
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField(new GUIContent(label), color, false, alpha, hdr, GUILayout.Height(30));
            EditorGUI.EndDisabledGroup();
        }


        private void DrawSingleAmbientLightFields(PaletteHolder paletteHolder)
        {
            EditorGUI.BeginChangeCheck();

            DrawEnvironmentLightSection("Ambient Light",
                _ambientLightTypeProp,
                _ambientLightValueProp,
                _ambientLightSaturationProp,
                _ambientLightColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                paletteHolder.SetAmbientLight();
            }
        }


        private void DrawTriAmbientLightFields(PaletteHolder paletteHolder)
        {
            EditorGUI.BeginChangeCheck();

            DrawEnvironmentLightSection("Ambient Sky Light", _ambientSkyLightTypeProp, _ambientSkyLightValueProp, _ambientSkyLightSaturationProp, _ambientSkyLightColorProp);
            DrawEnvironmentLightSection("Ambient Equator Light", _ambientEquatorLightTypeProp, _ambientEquatorLightValueProp, _ambientEquatorLightSaturationProp, _ambientEquatorLightColorProp);
            DrawEnvironmentLightSection("Ambient Ground Light", _ambientGroundLightTypeProp, _ambientGroundLightValueProp, _ambientGroundLightSaturationProp, _ambientGroundLightColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                paletteHolder.SetAmbientSkyLight();
                paletteHolder.SetAmbientEquatorLight();
                paletteHolder.SetAmbientGroundLight();
            }
        }


        /// <summary>
        ///     Draws a single ambient light section using the optimized property handling.
        /// </summary>
        private void DrawEnvironmentLightSection(string label, SerializedProperty typeProp, SerializedProperty valueProp, SerializedProperty saturationProp, SerializedProperty colorProp)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            DrawProperty(typeProp, label,
                () => (ColorType) EditorGUILayout.EnumPopup(label, (ColorType) typeProp.enumValueIndex),
                (prop, newValue) => prop.enumValueIndex = (int) newValue);

            DrawProperty(valueProp, $"{label} Value",
                () => EditorGUILayout.Slider("Value", valueProp.floatValue, ValueRange.x, ValueRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawProperty(saturationProp, $"{label} Saturation",
                () => EditorGUILayout.Slider("Saturation", saturationProp.floatValue, SaturationRange.x, SaturationRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawColorBox(colorProp.colorValue, false, true, "HDR Preview");
            GUILayout.EndVertical();
        }


        private void DrawSkyboxField(PaletteHolder paletteHolder)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            DrawProperty(_skyboxMaterialProp, "Skybox Material",
                () => skybox = (Material) EditorGUILayout.ObjectField("Skybox Material", _skyboxMaterialProp.objectReferenceValue, typeof(Material), false),
                (prop, newValue) => prop.objectReferenceValue = newValue);

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                paletteHolder.SetSkyboxMaterial();
            }
        }


        private void DrawSkyboxColorFields(PaletteHolder paletteHolder)
        {
            EditorGUI.BeginChangeCheck();

            DrawSkyboxColorSection("Sky Color", _skyColorTypeProp, _skyColorValueProp, _skyColorSaturationProp, _skyColorProp);
            DrawSkyboxColorSection("Horizon Color", _horizonColorTypeProp, _horizonColorValueProp, _horizonColorSaturationProp, _horizonColorProp);
            DrawSkyboxColorSection("Ground Color", _groundColorTypeProp, _groundColorValueProp, _groundColorSaturationProp, _groundColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                paletteHolder.SetSkyColor();
                paletteHolder.SetHorizonColor();
                paletteHolder.SetGroundColor();
            }
        }


        /// <summary>
        ///     Draws a single skybox color section using the optimized property handling.
        /// </summary>
        private void DrawSkyboxColorSection(string label, SerializedProperty typeProp, SerializedProperty valueProp, SerializedProperty saturationProp, SerializedProperty colorProp)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            DrawProperty(typeProp, label,
                () => (ColorType) EditorGUILayout.EnumPopup(label, (ColorType) typeProp.enumValueIndex),
                (prop, newValue) => prop.enumValueIndex = (int) newValue);

            DrawProperty(valueProp, $"{label} Value",
                () => EditorGUILayout.Slider("Value", valueProp.floatValue, ValueRange.x, ValueRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawProperty(saturationProp, $"{label} Saturation",
                () => EditorGUILayout.Slider("Saturation", saturationProp.floatValue, SaturationRange.x, SaturationRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawColorBox(colorProp.colorValue);

            GUILayout.EndVertical();
        }


        private void DrawRealtimeShadowFields(PaletteHolder paletteHolder)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();

            DrawProperty(_shadowColorTypeProp, "Shadow Color",
                () => (ColorType) EditorGUILayout.EnumPopup("Shadow Color", (ColorType) _shadowColorTypeProp.enumValueIndex),
                (prop, newValue) => prop.enumValueIndex = (int) newValue);

            DrawProperty(_shadowColorValueProp, "Shadow Value",
                () => EditorGUILayout.Slider("Value", _shadowColorValueProp.floatValue, ValueRange.x, ValueRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawProperty(_shadowColorSaturationProp, "Shadow Saturation",
                () => EditorGUILayout.Slider("Saturation", _shadowColorSaturationProp.floatValue, SaturationRange.x, SaturationRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawColorBox(_shadowColorProp.colorValue);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                paletteHolder.SetRealtimeShadowColor();
            }

            GUILayout.EndVertical();
        }


        private void DrawFogFields(PaletteHolder paletteHolder)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();

            DrawProperty(_fogColorTypeProp, "Fog Color",
                () => (ColorType) EditorGUILayout.EnumPopup("Fog Color", (ColorType) _fogColorTypeProp.enumValueIndex),
                (prop, newValue) => prop.enumValueIndex = (int) newValue);

            DrawProperty(_fogColorValueProp, "Fog Value",
                () => EditorGUILayout.Slider("Value", _fogColorValueProp.floatValue, ValueRange.x, ValueRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawProperty(_fogColorSaturationProp, "Fog Saturation",
                () => EditorGUILayout.Slider("Saturation", _fogColorSaturationProp.floatValue, SaturationRange.x, SaturationRange.y),
                (prop, newValue) => prop.floatValue = newValue);

            DrawColorBox(_fogColorProp.colorValue);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                paletteHolder.SetFogColor();
            }

            GUILayout.EndVertical();
        }
    }
}