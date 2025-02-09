using System;
using System.Collections.Generic;


namespace TextExtraTags {
    public class ParserBuffer {
        const int DefaultBufferSize = 128;


        int textSize;
        char[] textBuffer;
        List<ExtraTag> tags;

        public bool HasText => textSize > 0;
        public bool HasTags => tags.Count > 0;
        public bool Modified => HasText || HasTags;

        public ReadOnlySpan<char> Text => textBuffer.AsSpan(0, textSize);
        public IEnumerable<ExtraTag> Tags => tags;


        public ParserBuffer() {
            this.textSize = 0;
            this.textBuffer = new char[DefaultBufferSize];
            this.tags = new();
        }

        public ParserBuffer(int textCapacity, int tagsCapacity) {
            this.textSize = 0;
            this.textBuffer = new char[textCapacity];
            this.tags = new(tagsCapacity);
        }


        public void ClearAll() {
            textSize = 0;
            tags.Clear();
        }

        public void AddText(ReadOnlySpan<char> text) {
            Span<char> span = textBuffer.AsSpan(textSize);
            while (text.Length > span.Length) {
                int newBufferSize = textBuffer.Length * 2;
                var newBuffer = new char[newBufferSize];
                textBuffer.CopyTo(newBuffer, 0);

                textBuffer = newBuffer;
                span = textBuffer.AsSpan(textSize);
            }
            text.CopyTo(span);
            textSize += text.Length;
        }

        public void AddExtraTag(ExtraTag tag) {
            tags.Add(tag);
        }
    }
}
