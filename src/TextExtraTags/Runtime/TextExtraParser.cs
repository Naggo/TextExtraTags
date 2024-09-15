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

        public string parserName;
        public bool parseOnAwake;

        ExtraTagCollection _extraTags;
        ExtraTagCollection extraTags {
            get {
                if (_extraTags is null) {
                    _extraTags = new();
                }
                return _extraTags;
            }
        }
        public IReadOnlyExtraTagCollection ExtraTags => extraTags;

        TMP_Text _textComponent;
        public TMP_Text TextComponent {
            get {
                if (_textComponent is null) {
                    _textComponent = GetComponent<TMP_Text>();
                }
                return _textComponent;
            }
        }


        void Awake() {
            if (parseOnAwake) {
                Parse();
            }
        }

        void OnDestroy() {
            if (_extraTags is not null) {
                _extraTags.Clear();
            }
        }

        public Parser Parse() {
            var parser = ParserUtility.GetParser(parserName);
            var buffer = parser.Parse(sourceText, extraTags).AsArraySegment();
            TextComponent.SetCharArray(buffer.Array, buffer.Offset, buffer.Count);
            return parser;
        }
    }
}

#endif
