using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class TextExtraTagsSettings : ScriptableObject {
        public static readonly string DefaultPresetName = "Default";

        static TextExtraTagsSettings _instance;
        public static TextExtraTagsSettings Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<TextExtraTagsSettings>(nameof(TextExtraTagsSettings));
                    if (_instance == null) {
                        _instance = CreateDefaultSettings();
                    }
                }
                return _instance;
            }
        }

        public static TextExtraTagsSettings CreateDefaultSettings() {
            var settings = ScriptableObject.CreateInstance<TextExtraTagsSettings>();
            settings.defaultPreset = new ParserPreset(DefaultPresetName, 2);
            settings.parserPresets = new();

            return settings;
        }


        [SerializeField]
        ParserPreset defaultPreset;
        [SerializeField]
        List<ParserPreset> parserPresets;

        Dictionary<string, Parser> parsers;


        void OnDestroy() {
            ResetParsers();
        }


        public void ResetParsers() {
            if (parsers is null || parsers.Count == 0) return;
            parsers.Clear();
        }

        public Parser GetParser(string name, bool returnDefault = true) {
            Parser parser;
            if (parsers is null) {
                parsers = new();
            }
            if (parsers.TryGetValue(name, out parser)) {
                return parser;
            }
            var preset = GetPreset(name);
            if (preset is null) {
                if (returnDefault) {
                    return GetParser(DefaultPresetName, false);
                } else {
                    return null;
                }
            }
            parser = new Parser(preset);
            parsers[preset.Name] = parser;
            return parser;
        }

        public ParserPreset GetPreset(string name) {
            if (name == DefaultPresetName) {
                return defaultPreset;
            }
            foreach (var preset in parserPresets) {
                if (name == preset.Name) return preset;
            }
            return null;
        }

        public IEnumerable<string> GetNames() {
            yield return DefaultPresetName;
            foreach (var preset in parserPresets) {
                yield return preset.Name;
            }
        }
    }
}
