using System;
using UnityEngine;


namespace TextExtraTags.Editor {
    using UnityEditor;

    [CustomEditor(typeof(TextExtraTagsSettings), editorForChildClasses: false, isFallback=false)]
    public class TextExtraTagsSettingsEditor : Editor {
        SerializedProperty defaultPreset;
        SerializedProperty presets;

        TextExtraTagsSettings settings => target as TextExtraTagsSettings;


        void FindProperties() {
            defaultPreset = serializedObject.FindProperty("defaultPreset");
            presets = serializedObject.FindProperty("parserPresets");
        }


        void OnEnable() {
            FindProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawPreset(defaultPreset, true);
            int i = 0;
            foreach (SerializedProperty preset in presets) {
                DrawPreset(preset, false, i++);
            }
            DrawCreateButton();

            Apply();
        }

        void Apply() {
            if (serializedObject.ApplyModifiedProperties()) {
                settings.ResetAllParsers();
            }
        }

        void DrawPreset(SerializedProperty preset, bool isDefault, int index = 0) {
            SerializedProperty name = preset.FindPropertyRelative("name");
            SerializedProperty capacityLevel = preset.FindPropertyRelative("capacityLevel");
            SerializedProperty iterationLimit = preset.FindPropertyRelative("iterationLimit");
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
            EditorGUILayout.PropertyField(iterationLimit);
            EditorGUILayout.PropertyField(features);
            if (GUILayout.Button("Add Feature")) {
                ShowFeatureMenu(features);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        void ShowEditMenu(SerializedProperty preset, int index) {
            GenericMenu menu = new GenericMenu();

            if (index > 0) {
                void MoveUp() {
                    presets.MoveArrayElement(index, index-1);
                    Apply();
                }
                menu.AddItem(new GUIContent("Move Up"), false, MoveUp);
            } else {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < (presets.arraySize - 1)) {
                void MoveDown() {
                    presets.MoveArrayElement(index, index+1);
                    Apply();
                }
                menu.AddItem(new GUIContent("Move Down"), false, MoveDown);
            } else {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            void CallDuplicateCommand(object obj) {
                (obj as SerializedProperty)?.DuplicateCommand();
                Apply();
            }
            menu.AddItem(new GUIContent("Duplicate"), false, CallDuplicateCommand, preset);

            void CallDeleteCommand(object obj) {
                (obj as SerializedProperty)?.DeleteCommand();
                Apply();
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
                Apply();
            }

            var types = TypeCache.GetTypesDerivedFrom<ExtraTagFeature>();
            foreach (Type type in types) {
                string name = type.FullName.Replace('+', '.');
                menu.AddItem(new GUIContent(name), false, AddFeature, type);
            }

            menu.ShowAsContext();
        }

        void DrawCreateButton() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create new Parser")) {
                int index = presets.arraySize;
                presets.InsertArrayElementAtIndex(index);

                var preset = presets.GetArrayElementAtIndex(index);
                preset.FindPropertyRelative("name").stringValue = "New Parser";
                preset.FindPropertyRelative("capacityLevel").intValue = 1;
                preset.FindPropertyRelative("iterationLimit").intValue = 2;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
}
