using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ParserBuffer {
        int textSize;
        char[] textBuffer = new char[128];
        List<ExtraTagBase> tags = new();

        public bool HasText => textSize > 0;
        public bool HasTags => tags.Count > 0;
        public bool Modified => HasText || HasTags;

        public ReadOnlySpan<char> Text => textBuffer.AsSpan(0, textSize);
        public IEnumerable<ExtraTagBase> Tags => tags;


        public void ClearAll() {
            textSize = 0;
            tags.Clear();
        }

        public void AddText(ReadOnlySpan<char> text) {
            Span<char> span = textBuffer.AsSpan(textSize);
            while (text.Length > span.Length) {
                Array.Resize(ref textBuffer, textBuffer.Length * 2);
                span = textBuffer.AsSpan(textSize);
            }
            text.CopyTo(span);
            textSize += text.Length;
        }

        public void AddExtraTag(ExtraTagBase tag) {
            tags.Add(tag);
        }
    }
}
