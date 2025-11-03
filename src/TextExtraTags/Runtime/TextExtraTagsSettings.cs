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
                        _instance = ScriptableObject.CreateInstance<TextExtraTagsSettings>();
                    }
                }
                return _instance;
            }
        }


        [SerializeField]
        ParserPreset defaultPreset = new ParserPreset(DefaultPresetName);
        [SerializeField]
        List<ParserPreset> parserPresets = new();

        Parser defaultParser;
        Dictionary<string, Parser> parsers = new();


        void OnDestroy() {
            ResetAllParsers();
        }


        public void ResetAllParsers() {
            defaultParser = null;
            if (parsers is null || parsers.Count == 0) return;
            parsers.Clear();
        }

        public Parser GetParser(string name, bool returnDefault = true) {
            if (name == defaultPreset.Name) {
                return GetDefaultParser();
            }

            Parser parser;
            if (parsers.TryGetValue(name, out parser)) {
                return parser;
            }
            var preset = GetPreset(name);
            if (preset is null) {
                if (returnDefault) {
                    return GetDefaultParser();
                } else {
                    return null;
                }
            }
            parser = new Parser(preset);
            parsers[preset.Name] = parser;
            return parser;
        }

        public Parser GetDefaultParser() {
            if (defaultParser is null) {
                defaultParser = new Parser(defaultPreset);
            }
            return defaultParser;
        }

        public ParserPreset GetPreset(string name) {
            if (name == defaultPreset.Name) {
                return GetDefaultPreset();
            }

            foreach (var preset in parserPresets) {
                if (name == preset.Name) return preset;
            }
            return null;
        }

        public ParserPreset GetDefaultPreset() {
            return defaultPreset;
        }

        public IEnumerable<string> GetNames() {
            yield return defaultPreset.Name;
            foreach (var preset in parserPresets) {
                yield return preset.Name;
            }
        }
    }
}
