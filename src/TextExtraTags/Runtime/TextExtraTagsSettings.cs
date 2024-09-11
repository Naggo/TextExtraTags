using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class TextExtraTagsSettings : ScriptableObject {
        static TextExtraTagsSettings _instance;
        public static TextExtraTagsSettings Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<TextExtraTagsSettings>(nameof(TextExtraTagsSettings));
                    if (_instance == null) {
                        _instance = GetDefault();
                    }
                }
                return _instance;
            }
        }

        static TextExtraTagsSettings GetDefault() {
            var settings = ScriptableObject.CreateInstance<TextExtraTagsSettings>();
            settings.defaultPreset = new ParserPreset("default");
            settings.parserPresets = new();

            return settings;
        }


        public ParserPreset defaultPreset;
        public List<ParserPreset> parserPresets;
    }
}
