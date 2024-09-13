using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TextExtraTags.Editor {
    using UnityEditor;

    [CustomEditor(typeof(TextExtraTagsSettings), editorForChildClasses: false, isFallback=false)]
    public class TextExtraTagsSettingsEditor : Editor {

        SerializedProperty defaultPreset;
        SerializedProperty presets;

        TextExtraTagsSettings settings => target as TextExtraTagsSettings;

        void OnEnable() {
            defaultPreset = serializedObject.FindProperty("defaultPreset");
            presets = serializedObject.FindProperty("parserPresets");
        }

        public override void OnInspectorGUI() {
            // DrawDefaultInspector();

            DrawPreset(defaultPreset, true);
            int i = 0;
            foreach (SerializedProperty preset in presets) {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                DrawPreset(preset, false, i++);
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Create new Preset")) {
                int index = presets.arraySize;
                presets.InsertArrayElementAtIndex(index);

                var preset = presets.GetArrayElementAtIndex(index);
                preset.FindPropertyRelative("name").stringValue = "New Parser";
                preset.FindPropertyRelative("capacityLevel").intValue = 2;
            }

            if (serializedObject.ApplyModifiedProperties()) {
                settings.Parsers.Clear();
            }
        }

        void DrawPreset(SerializedProperty preset, bool isDefault, int index = 0) {
            SerializedProperty name = preset.FindPropertyRelative("name");
            SerializedProperty capacityLevel = preset.FindPropertyRelative("capacityLevel");
            SerializedProperty features = preset.FindPropertyRelative("features");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name.stringValue, EditorStyles.boldLabel);
            if (!isDefault && GUILayout.Button("Edit")) {
                ShowEditMenu(preset, index);
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            using (new EditorGUI.DisabledScope(isDefault)) {
                EditorGUILayout.PropertyField(name);
            }
            EditorGUILayout.PropertyField(capacityLevel);
            EditorGUILayout.PropertyField(features);
            if (GUILayout.Button("Add Feature")) {
                ShowFeatureMenu(features);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        void ShowEditMenu(SerializedProperty preset, int index) {
            GenericMenu menu = new GenericMenu();

            if (index > 0) {
                void MoveUp() {
                    presets.MoveArrayElement(index, index-1);
                }
                menu.AddItem(new GUIContent("Move Up"), false, MoveUp);
            } else {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < (presets.arraySize - 1)) {
                void MoveDown() {
                    presets.MoveArrayElement(index, index+1);
                }
                menu.AddItem(new GUIContent("Move Down"), false, MoveDown);
            } else {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            static void CallDuplicateCommand(object obj) {
                (obj as SerializedProperty)?.DuplicateCommand();
            }
            menu.AddItem(new GUIContent("Duplicate"), false, CallDuplicateCommand, preset);

            static void CallDeleteCommand(object obj) {
                (obj as SerializedProperty)?.DeleteCommand();
            }
            menu.AddItem(new GUIContent("Delete"), false, CallDeleteCommand, preset);

            menu.ShowAsContext();
        }

        void ShowFeatureMenu(SerializedProperty features) {
            GenericMenu menu = new GenericMenu();

            void AddFeature(object obj) {
                Type featureType = obj as Type;
                int index = features.arraySize;
                features.InsertArrayElementAtIndex(index);

                var feature = features.GetArrayElementAtIndex(index);
                feature.managedReferenceValue = Activator.CreateInstance(featureType);
            }

            var types = TypeCache.GetTypesDerivedFrom<ExtraTagFeature>();
            foreach (Type type in types) {
                string name = type.FullName.Replace('+', '.');
                menu.AddItem(new GUIContent(name), false, AddFeature, type);
            }

            menu.ShowAsContext();
        }
    }
}
