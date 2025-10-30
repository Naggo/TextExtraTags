#if TEXTEXTRATAGS_TEXTMESHPRO_SUPPORT

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TextExtraTags {
    [RequireComponent(typeof(TMP_Text))]
    public class TextExtraParser : MonoBehaviour {
        public TMP_Text textComponent;

        [TextArea(5, 10)]
        public string sourceText;

        public string parserName = TextExtraTagsSettings.DefaultPresetName;
        public bool parseOnAwake;

        ICollection<ExtraTag> _extraTags;
        ICollection<ExtraTag> extraTags {
            get {
                if (_extraTags is null) {
                    _extraTags = new List<ExtraTag>();
                }
                return _extraTags;
            }
        }


        void Awake() {
            if (parseOnAwake) {
                ParseAndSetText();
            }
        }

        void OnDestroy() {
            extraTags.Clear();
        }


        public Parser GetParser() {
            return ParserUtility.GetParser(parserName);
        }

        public Parser ParseText() {
            return ParseText(sourceText);
        }

        public Parser ParseText(ReadOnlySpan<char> source) {
            var parser = GetParser();
            return parser.Parse(source, extraTags);
        }

        public Parser ParseAndSetText() {
            return ParseAndSetText(sourceText);
        }

        public Parser ParseAndSetText(ReadOnlySpan<char> source) {
            var parser = ParseText(source);
            if (textComponent) {
                var buffer = parser.AsArraySegment();
                textComponent.SetCharArray(buffer.Array, buffer.Offset, buffer.Count);
            }
            return parser;
        }


        public void OverrideCollection(ICollection<ExtraTag> collection) {
            _extraTags = collection;
        }

        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTag {
            foreach (var item in extraTags) {
                if (item.Index == index && item is T result) {
                    tag = result;
                    return true;
                }
            }
            tag = default;
            return false;
        }

        public IEnumerable<T> GetExtraTags<T>(int index) where T: ExtraTag {
            foreach (var item in extraTags) {
                if (item.Index == index && item is T result) {
                    yield return result;
                }
            }
        }
    }
}

#endif
