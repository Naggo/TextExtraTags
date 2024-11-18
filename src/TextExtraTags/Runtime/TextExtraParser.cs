#if TEXTEXTRATAGS_TEXTMESHPRO_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TextExtraTags {
    [RequireComponent(typeof(TMP_Text))]
    public class TextExtraParser : MonoBehaviour {
        [TextArea(5, 10)]
        public string sourceText;

        public string parserName = TextExtraTagsSettings.DefaultPresetName;
        public bool parseOnAwake;

        ExtraTagCollection _mutableExtraTags;
        ExtraTagCollection mutableExtraTags {
            get {
                if (_mutableExtraTags is null) {
                    _mutableExtraTags = new();
                }
                return _mutableExtraTags;
            }
        }

        public IReadOnlyExtraTagCollection extraTags => mutableExtraTags;

        [System.Obsolete("Use extraTags.")]
        public IReadOnlyExtraTagCollection ExtraTags => extraTags;

        TMP_Text _textComponent;
        public TMP_Text textComponent {
            get {
                if (_textComponent is null) {
                    _textComponent = GetComponent<TMP_Text>();
                }
                return _textComponent;
            }
        }

        [System.Obsolete("Use textComponent.")]
        public TMP_Text TextComponent => textComponent;


        void Awake() {
            if (parseOnAwake) {
                Parse();
            }
        }

        void OnDestroy() {
            if (_mutableExtraTags is not null) {
                _mutableExtraTags.Clear();
            }
        }


        public Parser GetParser() {
            return ParserUtility.GetParser(parserName);
        }

        public Parser Parse() {
            var parser = GetParser();
            var buffer = parser.Parse(sourceText, mutableExtraTags).AsArraySegment();
            textComponent.SetCharArray(buffer.Array, buffer.Offset, buffer.Count);
            return parser;
        }
    }
}

#endif
