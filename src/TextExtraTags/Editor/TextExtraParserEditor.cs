#if TEXTEXTRATAGS_TEXTMESHPRO_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace TextExtraTags.Editor {
    using UnityEditor;

    [CustomEditor(typeof(TextExtraParser), editorForChildClasses: false, isFallback=false)]
    public class TextExtraParserEditor : Editor {
        static string[] parserNames;

        SerializedProperty textComponent;
        SerializedProperty sourceText;
        SerializedProperty parserName;
        SerializedProperty parseOnAwake;


        void FindProperties() {
            textComponent = serializedObject.FindProperty("textComponent");
            sourceText = serializedObject.FindProperty("sourceText");
            parserName = serializedObject.FindProperty("parserName");
            parseOnAwake = serializedObject.FindProperty("parseOnAwake");
        }


        public override void OnInspectorGUI() {
            if (sourceText == null) {
                FindProperties();
            }

            EditorGUILayout.PropertyField(textComponent);
            EditorGUILayout.PropertyField(sourceText);
            DrawParserName();
            EditorGUILayout.PropertyField(parseOnAwake);

            if (serializedObject.ApplyModifiedProperties()) {
                (target as TextExtraParser).ParseAndSetText();
            }
        }

        void DrawParserName() {
            string name = parserName.stringValue;

            string[] parserNames = ParserUtility.GetParserNames().ToArray();
            if (!ArrayUtility.Contains(parserNames, name)) {
                ArrayUtility.Insert(ref parserNames, 0, name);
            }

            DrawStringPopup(parserName.displayName, ref name, parserNames);
            parserName.stringValue = name;
        }

        void DrawStringPopup(string label, ref string current, string[] options) {
            int index = Array.IndexOf(options, current);
            index = EditorGUILayout.Popup(label, index, options);
            if (0 <= index && index < options.Length) {
                current = options[index];
            }
        }
    }
}

#endif
