using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace SOSXR.plet.EditorScripts
{
    /// <summary>
    ///     Custom inspector for <see cref="PletSceneSettings"/>. Draws the palette selector and collapsible
    ///     sections for skybox, ambient light, realtime shadow, and fog overrides, each with live preview
    ///     swatches and a "Get Palette Saturation and Value" sync button.
    /// </summary>
    [CustomEditor(typeof(PletSceneSettings))]
    public class PletSceneSettingsEditor : PaletteEditorBase
    {
        private SerializedProperty _paletteProp;
        private SerializedProperty _applySkyboxProp;
        private SerializedProperty _skyboxMaterialProp;

        // Skybox properties
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

        // Ambient light properties
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

        // Shadow properties
        private SerializedProperty _applyRealtimeShadowsProp;
        private SerializedProperty _realtimeShadowHueTypeProp;
        private SerializedProperty _realtimeShadowSaturationProp;
        private SerializedProperty _realtimeShadowValueProp;
        private SerializedProperty _realtimeShadowColorProp;


        // Fog properties
        private SerializedProperty _applyFogProp;
        private SerializedProperty _fogHueTypeProp;
        private SerializedProperty _fogSaturationProp;
        private SerializedProperty _fogValueProp;
        private SerializedProperty _fogColorProp;


        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            // Main settings
            _paletteProp = serializedObject.FindProperty(nameof(PletSceneSettings.Palette));
            _applySkyboxProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplySkybox));
            _skyboxMaterialProp = serializedObject.FindProperty(nameof(PletSceneSettings.SkyboxMaterial));

            // Skybox
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

            // Ambient light
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

            // Shadows
            _applyRealtimeShadowsProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplyRealtimeShadows));
            _realtimeShadowHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowHueType));
            _realtimeShadowSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowSaturation));
            _realtimeShadowValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowValue));
            _realtimeShadowColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.RealtimeShadowColor));

            // Fog
            _applyFogProp = serializedObject.FindProperty(nameof(PletSceneSettings.ApplyFog));
            _fogHueTypeProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogHueType));
            _fogSaturationProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogSaturation));
            _fogValueProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogValue));
            _fogColorProp = serializedObject.FindProperty(nameof(PletSceneSettings.FogColor));
        }


        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                EditorGUILayout.HelpBox("Target is null.", MessageType.Error);

                return;
            }

            serializedObject.Update();

            var pletSceneSettings = target as PletSceneSettings;

            if (pletSceneSettings == null)
            {
                EditorGUILayout.HelpBox("Target is not a PletSceneSettings.", MessageType.Error);

                return;
            }

            if (!IsActiveSceneSettings(pletSceneSettings))
            {
                DrawInactiveSettingsWarning();

                return;
            }

            if (_paletteProp.objectReferenceValue == null)
            {
                DrawCreatePaletteButton();
            }

            if (!DrawPaletteSelector())
            {
                return;
            }

            if (GUILayout.Button(ButtonText + " for all " + nameof(ColorProvider) + "s in the scene"))
            {
                pletSceneSettings.GetColorProvidersSVFromPalette();
            }

            var palette = (Palette)_paletteProp.objectReferenceValue;

            EditorGUILayout.Space();
            DrawSectionHeader("Palette Colors");
            DrawPaletteColorFields(palette);
            EditorGUILayout.Space(10);

            DrawSkyboxSection(pletSceneSettings);
            DrawAmbientLightSection(pletSceneSettings);
            DrawRealtimeShadowsSection(pletSceneSettings);
            DrawFogSection(pletSceneSettings);


            if (serializedObject.ApplyModifiedProperties())
            {
                Repaint();
            }
        }


        private void DrawCreatePaletteButton()
        {
            if (GUILayout.Button("Create Palette"))
            {
                var palette = CreateInstance<Palette>();
                palette.name = "Palette";

                var assetPath = Path.Combine(PletHelpers.FolderPath, palette.name + ".asset");
                AssetDatabase.CreateAsset(palette, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var pletSceneSettings = (PletSceneSettings)target;
                pletSceneSettings.Palette = AssetDatabase.LoadAssetAtPath<Palette>(assetPath);
            }
        }


        private bool IsActiveSceneSettings(PletSceneSettings settings)
        {
            return PletHelpers.GetPletSceneSettings() == settings;
        }


        private void DrawInactiveSettingsWarning()
        {
            EditorGUILayout.HelpBox(
                "This Palette Holder is not being used in the scene. It will not apply any colors. " +
                "Please check the following:\n" +
                "1. It is either the only PaletteHolder in any Resources folder\n" +
                "2. It is one of multiple PaletteHolders in any Resources folder, but has the exact name of the current scene\n" +
                "3. (In case of 2) It is the only PaletteHolder with the name of the scene\n" +
                "4. You are currently in the scene where this PaletteHolder belongs",
                MessageType.Warning);
        }


        private bool DrawPaletteSelector()
        {
            EditorGUILayout.PropertyField(_paletteProp);

            return _paletteProp.objectReferenceValue != null;
        }


        private void DrawSkyboxSection(PletSceneSettings pletSceneSettings)
        {
            if (DrawToggleHeader("Skybox", "Apply Skybox", _applySkyboxProp))
            {
                DrawSkyboxMaterialField(pletSceneSettings);

                if (_skyboxMaterialProp.objectReferenceValue != null)
                {
                    DrawSkyboxColorFields(pletSceneSettings);
                }
                else
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.HelpBox("Please assign a Skybox Material to apply colors.", MessageType.Warning);
                }
            }
        }


        private void DrawAmbientLightSection(PletSceneSettings pletSceneSettings)
        {
            EditorGUILayout.Space(10);

            if (RenderSettings.ambientMode == AmbientMode.Skybox)
            {
                EditorGUILayout.HelpBox(
                    "Ambient Light is disabled because the Environment Lighting is set to Skybox in Lighting - Environment",
                    MessageType.Info);

                return;
            }

            if (DrawToggleHeader("Ambient Light", "Apply Ambient Light Colors", _applyAmbientLightProp))
            {
                if (RenderSettings.ambientMode == AmbientMode.Flat)
                {
                    DrawFlatAmbientLightFields(pletSceneSettings);
                }
                else if (RenderSettings.ambientMode == AmbientMode.Trilight)
                {
                    DrawTriAmbientLightFields(pletSceneSettings);
                }
            }
        }


        private void DrawRealtimeShadowsSection(PletSceneSettings pletSceneSettings)
        {
            EditorGUILayout.Space(10);

            if (DrawToggleHeader("Realtime Shadows", "Apply Realtime Shadow Color", _applyRealtimeShadowsProp))
            {
                DrawRealtimeShadowFields(pletSceneSettings);
            }
        }


        private void DrawFogSection(PletSceneSettings pletSceneSettings)
        {
            EditorGUILayout.Space(10);

            if (!RenderSettings.fog)
            {
                EditorGUILayout.HelpBox("Fog is disabled in Lighting - Environment", MessageType.Info);

                return;
            }

            if (DrawToggleHeader("Fog", "Apply Fog Color", _applyFogProp))
            {
                DrawFogFields(pletSceneSettings);
            }
        }


        private bool DrawToggleHeader(string label, string toggleLabel, SerializedProperty toggleProp)
        {
            EditorGUILayout.BeginHorizontal();
            DrawSectionHeader(label);
            toggleProp.boolValue = EditorGUILayout.ToggleLeft(toggleLabel, toggleProp.boolValue);
            EditorGUILayout.EndHorizontal();

            return toggleProp.boolValue;
        }


        private void DrawSectionHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }


        private void DrawPaletteColorFields(Palette palette)
        {
            const int height = 50;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Base, false, false, $"{nameof(HueType)} - {nameof(HueType.Base)}", height);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Tone, false, false, $"{nameof(HueType)} - {nameof(HueType.Tone)}", height);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawColorBox(palette.Accent, false, false, $"{nameof(HueType)} - {nameof(HueType.Accent)}", height);
            EditorGUILayout.EndVertical();
        }


        private void DrawSkyboxMaterialField(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            DrawProperty(_skyboxMaterialProp, "Skybox Material",
                () => (Material)EditorGUILayout.ObjectField("Skybox Material",
                    _skyboxMaterialProp.objectReferenceValue, typeof(Material), false),
                (prop, newValue) => prop.objectReferenceValue = newValue);

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetSkyboxMaterial();
            }
        }


        private void DrawSkyboxColorFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Skybox Sky", _skyboxSkyHueTypeProp, _skyboxSkyValueProp,
                _skyboxSkySaturationProp, _skyboxSkyColorProp);

            DrawSection("Skybox Horizon", _skyboxHorizonHueTypeProp, _skyboxHorizonValueProp,
                _skyboxHorizonSaturationProp, _skyboxHorizonColorProp);

            DrawSection("Skybox Ground", _skyboxGroundHueTypeProp, _skyboxGroundValueProp,
                _skyboxGroundSaturationProp, _skyboxGroundColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetAllSkyboxAndLights();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetSkyboxSVFromPalette();
            }
        }


        private void DrawFlatAmbientLightFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Ambient Light", _ambientLightHueTypeProp, _ambientLightValueProp, _ambientLightSaturationProp, _ambientLightColorProp, null, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetAllSkyboxAndLights();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetAmbientLightSVFromPalette();
            }
        }


        private void DrawTriAmbientLightFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Ambient Sky Light", _ambientSkyLightTypeProp, _ambientSkyLightValueProp,
                _ambientSkyLightSaturationProp, _ambientSkyLightColorProp, null, true);

            DrawSection("Ambient Equator Light", _ambientEquatorLightTypeProp, _ambientEquatorLightValueProp,
                _ambientEquatorLightSaturationProp, _ambientEquatorLightColorProp, null, true);

            DrawSection("Ambient Ground Light", _ambientGroundLightTypeProp, _ambientGroundLightValueProp,
                _ambientGroundLightSaturationProp, _ambientGroundLightColorProp, null, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetAllSkyboxAndLights();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetAmbientLightSVFromPalette();
            }
        }


        private void DrawRealtimeShadowFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Realtime Shadow", _realtimeShadowHueTypeProp, _realtimeShadowValueProp,
                _realtimeShadowSaturationProp, _realtimeShadowColorProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                pletSceneSettings.SetRealtimeShadowColor();
            }

            if (GUILayout.Button(ButtonText))
            {
                pletSceneSettings.GetRealtimeShadowSVFromPalette();
            }
        }


        private void DrawFogFields(PletSceneSettings pletSceneSettings)
        {
            EditorGUI.BeginChangeCheck();

            DrawSection("Fog", _fogHueTypeProp, _fogValueProp, _fogSaturationProp, _fogColorProp);

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
