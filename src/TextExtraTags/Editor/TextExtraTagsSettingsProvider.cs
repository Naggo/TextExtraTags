using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace TextExtraTags.Editor {
    using UnityEditor;

    public class TextExtraTagsSettingsProvider : SettingsProvider {
        // original: https://qiita.com/sune2/items/a88cdee6e9a86652137c

        const string settingPath = "Project/Text Extra Tags";


        [SettingsProvider]
        public static SettingsProvider CreateProvider() {
            // SettingsScope を Project にします
            return new TextExtraTagsSettingsProvider(settingPath, SettingsScope.Project, null);
        }

        static void SaveSettingsAsset(TextExtraTagsSettings settings) {
            var parent = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(parent)) {
                // Resources フォルダが無いことを考慮
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            var assetPath = Path.Combine(parent, Path.ChangeExtension(nameof(TextExtraTagsSettings), ".asset"));
            AssetDatabase.CreateAsset(settings, assetPath);
        }


        Editor _editor;


        public TextExtraTagsSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords)
            : base(path, scopes, keywords) {}


        public override void OnGUI(string searchContext) {
            if (_editor?.target == null) {
                Editor.CreateCachedEditor(TextExtraTagsSettings.Instance, null, ref _editor);
            }

            var instance = _editor.target as TextExtraTagsSettings;
            bool isPersistent = EditorUtility.IsPersistent(instance);
            if (isPersistent) {
                EditorGUILayout.LabelField("Asset Path:", AssetDatabase.GetAssetPath(instance));
            } else {
                if (GUILayout.Button("Create Asset")) {
                    instance = TextExtraTagsSettings.Instance;
                    SaveSettingsAsset(instance);
                    Editor.CreateCachedEditor(instance, null, ref _editor);
                }
            }

            using (new EditorGUI.DisabledScope(!isPersistent)) {
                EditorGUILayout.Space();
                _editor.OnInspectorGUI();
            }
        }
    }
}
